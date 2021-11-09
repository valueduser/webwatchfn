#!/bin/bash

CONFIGFILE="./config"

eval $(sed '/:/!d;/^ *#/d;s/:/ /;' < "$CONFIGFILE" | while read -r key val
do
    str="$key='$val'"
    echo "$str"
done)

dt=$(date '+%Y%m%d%H%M');

resourceGroupName="webwatcher"$dt
storageAccountName="blob"$dt
fnStorageAccountName="fnsto"$dt
functionAppName="watch"$dt
functionContainer="function"$dt
keyvaultName="kv"$dt
blobName="blob"$dt
containerName="container"$dt
servicePrincipalName="sp"$dt

echo "Creating Azure resources..."

az login --service-principal -u $appID --password $password --tenant $tenant
echo "Creating a service principal..."
sp=`az ad sp create-for-rbac --name $resourceGroupName"sp"`

az account set --subscription $subscription

echo "Creating resource group" $resourceGroupName"..."
az group create --name $resourceGroupName --location $location

echo "Creating key vault" $keyvaultName"..."
az keyvault create \
  --resource-group $resourceGroupName \
  --name $keyvaultName \
  --location $location \
  --sku "standard" \
  --subscription $subscription

echo "Creating storage account" $storageAccountName"..."
az storage account create  \
  --name $storageAccountName  \
  --resource-group $resourceGroupName \
  --access-tier "Cool" \
  --kind "BlobStorage" \
  --location $location \
  --sku "Standard_LRS" \
  --subscription $subscription

echo "Retrieving connection string..."
connectionString=`az storage account show-connection-string --name $storageAccountName --resource-group $resourceGroupName --output tsv`

echo "Populating key vault..."
az keyvault secret set \
  --vault-name $keyvaultName \
  --name "EmailTo" \
  --value $emailTo

az keyvault secret set \
  --vault-name $keyvaultName \
  --name "SendGridApiKey" \
  --value $sendGridKey

az keyvault secret set \
  --vault-name $keyvaultName \
  --name "BlobConnectionString" \
  --value $connectionString

az keyvault secret set \
  --vault-name $keyvaultName \
  --name "ContainerName" \
  --value $containerName

echo "Creating storage container" $containerName"..."
az storage container create \
  --name $containerName \
  --public-access off \
  --connection-string $connectionString

echo "Uploading to blob storage" $blobName"..."
az storage blob upload --container-name $containerName \
  --file $pathToFile\
  --connection-string $connectionString \
  --name $"websites.json"

echo "Creating function app storage account" $fnStorageAccountName"..."
az storage account create  \
  --name $fnStorageAccountName  \
  --resource-group $resourceGroupName \
  --location $location \
  --sku Standard_LRS \
  --subscription $subscription

echo "Creating function app" $functionAppName"..."
az functionapp create \
  --name $functionAppName  \
  --storage-account $fnStorageAccountName  \
  --consumption-plan-location $location \
  --runtime dotnet \
  --resource-group $resourceGroupName \
  --functions-version 4

echo "Building and publishing the function project" $functionProjectLocation"/WebWatcher.csproj..."
dotnet build $functionProjectLocation --configuration Release
cd $functionProjectLocation"/bin/Release/net6.0/"
zip -r $functionAppName.zip * 

az storage container create \
  --name $functionContainer \
  --public-access off \
  --connection-string $connectionString

az storage container create \
  --name $functionContainer \
  --public-access off \
  --connection-string $connectionString

echo "Uploading function zip to blob"
az storage blob upload --container-name $functionContainer \
  --file $functionAppName.zip \
  --connection-string $connectionString \
  --name $functionAppName.zip

format='+%Y-%m-%dT%H:%M:%SZ'
start=`date -v-24H ${format}`
end=`date -v+8760H ${format}`

sas=`az storage account generate-sas \
  --permissions rl \
  --connection-string $connectionString \
  --expiry $end \
  --start $start \
  --resource-types sco \
  --services b \
  --output tsv`
sas="${sas//%3A/:}"
  
echo "Cleaning up "${functionAppName}".zip..."
rm ${functionAppName}.zip

packageLocation="https://$storageAccountName.blob.core.windows.net/$functionContainer/$functionAppName.zip?$sas"

echo "Parsing keyvault service principal..."
clientId=`cut -d "," -f 3 <<< $sp`
clientId=`cut -d ":" -f 2 <<< $clientId`
clientSecret=`cut -d "," -f 4 <<< $sp`
clientSecret=`cut -d ":" -f 2 <<< $clientSecret`
echo "Updating the app settings..."
az functionapp config appsettings set \
  --name $functionAppName \
  --resource-group $resourceGroupName \
  --settings "WEBSITE_RUN_FROM_PACKAGE=$packageLocation TenantId=$tenant ClientId=$clientId ClientSecret=$clientSecret KeyVaultUri=$keyvaultName"

echo "Done!"
