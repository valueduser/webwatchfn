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
keyvaultName="kv"$dt
blobName="blob"$dt
containerName="container"$dt
servicePrincipalName="sp"$dt

echo "Creating Azure resources..."

az login --service-principal -u $appID --password $password --tenant $tenant

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
connectionString=$(az storage account show-connection-string --name $storageAccountName --resource-group $resourceGroupName --subscription $subscription --output tsv)

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

echo "Creating function app" $functionAppName
az storage account create  \
  --name $fnStorageAccountName  \
  --resource-group $resourceGroupName \
  --location $location \
  --sku Standard_LRS \
  --subscription $subscription
az functionapp create \
  --name $functionAppName  \
  --storage-account $fnStorageAccountName  \
  --consumption-plan-location $location \
  --runtime dotnet \
  --resource-group $resourceGroupName 

echo "Building and publishing the function project" $functionProjectLocation"/WebWatcher.csproj..."
dotnet build $functionProjectLocation"/WebWatcher.csproj" --configuration Release
dotnet publish $functionProjectLocation"/WebWatcher.csproj" --configuration Release
cd $functionProjectLocation"/bin/Release/netcoreapp2.1"
zip -r ${functionAppName}.zip *

echo "Deploying function project..."
az functionapp deployment source config-zip \
  --name $functionAppName \
  --resource-group $resourceGroupName \
  --src $functionAppName.zip
  
echo "Cleaning up "${functionAppName}".zip..."
rm ${functionAppName}.zip

az functionapp config appsettings set \
  --name $functionAppName \
  --resource-group $resourceGroupName \
  --settings AzureKeyVault:ClientId=$appID AzureKeyVault:ClientSecret=$password AzureKeyVault:VaultName=$keyvaultName WEBSITE_RUN_FROM_PACKAGE=1

echo "Done!"
