#!/usr/bin/env bash

echo "Forcing .NET MAUI iOS build detection..."

# Vérifier la version de .NET installée
dotnet --version

# Nettoyer le cache des builds précédents
dotnet clean SmartPharma5.sln

# Restaurer les dépendances du projet
dotnet restore SmartPharma5.sln

# Compiler le projet iOS
dotnet build SmartPharma5.sln -c Release -f net7.0-ios
