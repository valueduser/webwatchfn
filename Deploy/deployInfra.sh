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
functionAppName="watch"$dt
keyvaultName="kv"$dt
blobName="blob"$dt
containerName="container"$dt

echo "Creating Azure resources..."

az login --service-principal -u $appID --password $password --tenant $tenant

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
az functionapp create \
  --name $functionAppName  \
  --storage-account $storageAccountName  \
  --consumption-plan-location $location \
  --resource-group $resourceGroupName 

# az functionapp config appsettings set \
#   --name $functionAppName \
#   --resource-group $resourceGroupName \
#   --settings StorageConStr=$connectionString

# publish the code
# dotnet publish -c Release
# $publishFolder = "FunctionsDemo/bin/Release/netcoreapp2.1/publish"

# create the zip
# $publishZip = "publish.zip"
# if(Test-path $publishZip) {Remove-item $publishZip}
# Add-Type -assembly "system.io.compression.filesystem"
# [io.compression.zipfile]::CreateFromDirectory($publishFolder, $publishZip)

# deploy the zipped package
# az functionapp deployment source config-zip \
  # --name $functionAppName \
  # --resource-group $resourceGroupName \
#  --src $publishZip

