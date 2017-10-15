# Cryptography.AesCrypt
C# class wrapping the .Net Cryptography Aes class, incorporating compression and base64 wrapped string keys in a nice little package with through unit testing

## License
GNU License

Copyright(c)2013, Jillian England

## Usage
Interface's are included for use with dependancy injection.

Encrypt or Decrypt byte arrays or strings WITH data compression before encryption.

Key can be passed and set either in the method or a key can be set in the constructor and the instance remembers the key for all subsequent encryptions/decryptions.

Every new message encryption call gets it's own inititalizaion vector which is saved as part of the returned encrypted message.

Error checking throws a specific error when an incorrect key is used for decryption.

## Error checking
If you forget to set a key in the constructor AND you do not call the method with an encryption key an appropriate error message is generated.

If you try to decrypt a message with an incorrect key and appropriate error message is generated.

## Usage that retruns encrypted base64 string using predefined key

All keys are required to be base64 encoded and are retruned base64 encoded.
Encrypt->Decrypt example with pre-generated key. Returns base64 encrypted string.
```
var crypto = new AesCryptString();

const string key = "Q+pJALEKLqzBYWrctPUz64DXZK9zEQW5NuWOtSdPeCM=";

var cipher = crypto.Encrypt(key, plain);
var plain2 = crypto.Decrypt(key, cipher);

OR

var crypto = new AesCryptString("Q+pJALEKLqzBYWrctPUz64DXZK9zEQW5NuWOtSdPeCM=");

var crypt = crypto.Encrypt(plain);
var plain2 = crypto.Decrypt(crypt);

```
Just a reminder if you need a new AesKey for a new message you can use the .Net librarys like this;
```
var keyGenerator = new AesManaged();

keyGenerator.GenerateKey();
var key1 = Convert.ToBase64String(keyGenerator.Key);

```

