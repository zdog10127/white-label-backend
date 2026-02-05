#!/bin/bash

echo "🚀 Iniciando build para AWS Elastic Beanstalk..."

# Limpar publicações anteriores
echo "🧹 Limpando arquivos antigos..."
rm -rf publish/
rm -f aws-deploy.zip

# Restaurar dependências
echo "📦 Restaurando dependências..."
dotnet restore

# Publicar aplicação
echo "🔨 Compilando aplicação..."
dotnet publish -c Release -o publish/

# Copiar configurações do Elastic Beanstalk
echo "📋 Copiando configurações AWS..."
if [ -d ".ebextensions" ]; then
    cp -r .ebextensions publish/
fi

# Criar arquivo ZIP para deploy
echo "📦 Criando pacote de deploy..."
cd publish
zip -r ../aws-deploy.zip .
cd ..

echo "✅ Pacote criado com sucesso: aws-deploy.zip"
ls -lh aws-deploy.zip