import argparse
import json
import sys
import urllib.request


API_ENDPOINT_URL_TEMPLATE = 'https://api.telegram.org/bot{tg_token}/setWebhook'
FUNCTION_URL_TEMPLATE = 'https://functions.yandexcloud.net/{function_id}'


def parse_args(raw_args):
    parser = argparse.ArgumentParser()

    parser.add_argument(
        '--telegram-token',
        help='Token for target Telegram bot to set webhook',
        required=True,
        type=str,
    )
    parser.add_argument(
        '--function-id',
        help='ID of YC Function to set as receiver of webhook',
        required=True,
        type=str,
    )
    args = parser.parse_args(raw_args)
    return args


def set_webhook(tg_token, function_id):
    data = {
        'url': FUNCTION_URL_TEMPLATE.format(function_id=function_id),
    }
    raw_data = json.dumps(data).encode('utf8')

    request = urllib.request.Request(
        API_ENDPOINT_URL_TEMPLATE.format(tg_token=tg_token),
        method='POST',
        data=raw_data,
        headers={'content-type': 'application/json'}
    )
    response = urllib.request.urlopen(request)
    return response.read().decode('utf8')


def main(raw_args):
    args = parse_args(raw_args)
    tg_api_response = set_webhook(args.telegram_token, args.function_id)
    print(tg_api_response)


if __name__ == '__main__':
    main(sys.argv[1:])
