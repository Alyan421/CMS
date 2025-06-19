#!/bin/bash
set -e

# Build Angular app
cd cms.client
npm install
npm run build --configuration production

# Move Angular dist files to wwwroot
cd ..
mkdir -p CMS.Server/wwwroot
cp -r cms.client/dist/cms.client/* CMS.Server/wwwroot/

# Build .NET app
cd CMS.Server
dotnet publish -c Release

# Remove EF migrations for render deployment
# dotnet ef database update