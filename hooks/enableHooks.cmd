curl ^
  --request POST ^
  --url https://api.telegram.org/bot<TG_TOKEN>/setWebhook ^
  --header "content-type: application/json" ^
  --data "{\"url\": \"https://functions.yandexcloud.net/<FUNCTION_ID>/<FUNCTION_NAME>\"}"