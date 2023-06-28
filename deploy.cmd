rmdir /S /Q BotTemplate\bin
yc serverless function version create --function-name <FUNCTION_NAME> --entrypoint BotTemplate.TelegramHandler --runtime dotnet6 --service-account-id <SERVICE_ACCOUNT_ID> --execution-timeout 5s --source-path BotTemplate
