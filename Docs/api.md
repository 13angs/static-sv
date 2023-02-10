# Documents

## Create or upload new image

### Headers

```json
{
    "x-static-signature": "the_calculated_signature"
}
```

### Request body

```json
{
    "type": "image/jpeg",
    "name": "tiny  image",
    "base64_encoded_file": "base64_image"
}
```

### Responses

Success

```bash
{
    "image_url": "http://localhost:5000/wwwroot/images/tiny-image_2023-02-09_14-35-27.jpeg",
    "signature": "y84O6qi9WK/mcXFpA3uMHt8qMSrFwDsJnxFwx0o40cA=",
    "error_code": "SUCCESS"
}
```

Failed

```json
{
    "status_code": 401,
    "message": "Invalid signature",
    "errors": [
        {
            "message": "y84O6qi9WK/mcXFpA3uMHt8qMSrFwDsJnxFwx0o40cA=",
            "field": "server_signature"
        },
        {
            "message": "YonVLfwLZxsx1S0U0ayZsWlLJwb3SahkSBSfJqf+G+8=",
            "field": "client_signature"
        },
        {
            "message": "{\"type\":\"image/jpeg\",\"name\":\"tiny  image\"}",
            "field": "server_req_body"
        }
    ]
}
```

## Delete existing image

### Headers

```json
{
    "x-static-signature": "the_calculated_signature"
}
```

### Request body

```json
{
    "url": "http://localhost:5000/wwwroot/images/tiny-image_2023-02-09_14-35-27.jpeg/"
}
```

### Responses

Success

```bash
Empty body
```

Failed

```json
{
    "status_code": 400,
    "message": "image name: tiny-image_2023-02-09_14-35-27.jpeg doesn't exist",
    "errors": [
        {
            "message": "/app/wwwroot/images/tiny-image_2023-02-09_14-35-27.jpeg",
            "field": "full_name"
        },
        {
            "message": "http://localhost:5000/wwwroot/images/tiny-image_2023-02-09_14-35-27.jpeg/",
            "field": "url"
        }
    ]
}
```