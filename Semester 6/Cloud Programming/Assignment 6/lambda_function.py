import json
import boto3
import qrcode
import io
from datetime import datetime

def lambda_handler(event, context):
    try:
        body = json.loads(event.get("body", "{}"))
        url = body.get("url")
        if not url:
            return {
                'statusCode': 400,
                'body': json.dumps("Missing 'url' in request")
            }
        
        # Generowanie nazwy pliku
        now = datetime.now()
        file_name = "qr" + now.strftime("%H%M%S") + f"-{now.microsecond:06d}" + ".png"

        # Tworzenie kodu QR
        qr = qrcode.QRCode(
            version=1,
            error_correction=qrcode.constants.ERROR_CORRECT_L,
            box_size=10,
            border=4,
        )
        qr.add_data(url)
        qr.make(fit=True)
        img = qr.make_image(fill_color="black", back_color="white")

        # Buforowanie obrazu do pamięci
        img_bytes = io.BytesIO()
        img.save(img_bytes, format='PNG')
        img_bytes.seek(0)

        # Wysyłanie do S3
        bucket_name = 'qr-codes-7t98nm5y43v789'
        region = 'us-east-1'
        s3 = boto3.client('s3', region_name=region)
        s3.upload_fileobj(
            img_bytes,
            bucket_name,
            file_name,
            ExtraArgs={'ContentType': 'image/png'}
        )

        # Budowa publicznego URL
        public_url = f"https://{bucket_name}.s3.{region}.amazonaws.com/{file_name}"

        return {
            'statusCode': 200,
            'body': json.dumps({'qr_url': public_url})
        }

    except Exception as e:
        return {
            'statusCode': 500,
            'body': json.dumps(f"Error: {str(e)}")
        }
