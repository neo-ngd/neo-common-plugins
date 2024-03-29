**Request**

```json
{
    "network_identifier": {
        "blockchain": "neo n3",
        "network": "testnet"
    },
    "unsigned_transaction": "ALRzrxGkOQ8AAAAAADSVAwAAAAAA88wVAAK5eNCL546SGQS2/lr79rjoWq9gnQHfNbqO/8twxYe7jr\u002Bt8IdvXkdN6AEAXgwHcm9zZXR0YREMFCaYbl30TI57vj\u002Bgp119JoxTiJZCDBTfNbqO/8twxYe7jr\u002Bt8IdvXkdN6BTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1ICACgMIQNgpkd/JmKLheTo22NOCBMwXuJ9eH5dVbX33XUjgO47\u002B0FW57MnACgMIQKw4cVA3wJMYxCqoePYVa2fzodCbSRdt1/qNE41kTyPzkFW57Mn",
    "signatures": [
        {
            "hex_bytes": "d6d6aa6aeaff97ff63b43429979ff8a4366f6f1e7d618e33a19b6497adea551c9bd49640ad01e59a200a1c388339d6515dfe15e21dedae43aa8e37372cee3daa",
            "signing_payload": {
                "address": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw",
                "hex_bytes": "b1982e918ec252b6b0687eed95e774804337380df1e7095443b16cc9db34444a",
                "account_identifier": {
                    "address": "NcpevbKeAFioFRs8zdB5J9UkMZQxKBwExw"
                },
                "signature_type": "ecdsa"
            },
            "public_key": {
                "hex_bytes": "0460a6477f26628b85e4e8db634e0813305ee27d787e5d55b5f7dd752380ee3bfb87d23c2bf9f7f2cd4c3f8e0dcdfc3846b9b902da26bc970e01732e06b6101ae7",
                "curve_type": "secp256r1"
            },
            "signature_type": "ecdsa"
        },
        {
            "hex_bytes": "7a7e3e29133990327440b390ee44acea4768f0367dd4adc8335d6a4cd84b05ae2b6fabb9649dc0099291b2da9a8c8c9166528e37bbdf6fc4574c861127a1c2fd",
            "signing_payload": {
                "address": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F",
                "hex_bytes": "b1982e918ec252b6b0687eed95e774804337380df1e7095443b16cc9db34444a",
                "account_identifier": {
                    "address": "NgGCEfyNoSaRZYxoj6WdJqkZnov3DAaY6F"
                },
                "signature_type": "ecdsa"
            },
            "public_key": {
                "hex_bytes": "04b0e1c540df024c6310aaa1e3d855ad9fce87426d245db75fea344e35913c8fce11ac4ce19c1af2f566039c6a76b05ea25fe746602cbfcefd278b24d031a19020",
                "curve_type": "secp256r1"
            },
            "signature_type": "ecdsa"
        }
    ]
}
```

**Response**

```json
{
    "signed_transaction": "ALRzrxGkOQ8AAAAAADSVAwAAAAAA88wVAAK5eNCL546SGQS2/lr79rjoWq9gnQHfNbqO/8twxYe7jr\u002Bt8IdvXkdN6AEAXgwHcm9zZXR0YREMFCaYbl30TI57vj\u002Bgp119JoxTiJZCDBTfNbqO/8twxYe7jr\u002Bt8IdvXkdN6BTAHwwIdHJhbnNmZXIMFPVj6kC8KD1NDgXEjqMFs/Kgc0DvQWJ9W1ICQgxA1taqaur/l/9jtDQpl5/4pDZvbx59YY4zoZtkl63qVRyb1JZArQHlmiAKHDiDOdZRXf4V4h3trkOqjjc3LO49qigMIQNgpkd/JmKLheTo22NOCBMwXuJ9eH5dVbX33XUjgO47\u002B0FW57MnQgxAen4\u002BKRM5kDJ0QLOQ7kSs6kdo8DZ91K3IM11qTNhLBa4rb6u5ZJ3ACZKRstqajIyRZlKON7vfb8RXTIYRJ6HC/SgMIQKw4cVA3wJMYxCqoePYVa2fzodCbSRdt1/qNE41kTyPzkFW57Mn"
}
```
