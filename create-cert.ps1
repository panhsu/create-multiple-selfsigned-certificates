# 1. 產生 root CA
$root = New-SelfSignedCertificate -Subject "CN=TestRootCA" -KeyExportPolicy Exportable -CertStoreLocation "Cert:\CurrentUser\My" -KeyUsage CertSign,CRLSign,DigitalSignature -Type Custom -KeyAlgorithm RSA -KeyLength 2048

# 匯出 rootCA.crt
Export-Certificate -Cert $root -FilePath .\rootCA.crt

# 2. 產生 server certificate
$server = New-SelfSignedCertificate -DnsName "localhost" -CertStoreLocation "Cert:\CurrentUser\My" -Signer $root -KeyExportPolicy Exportable
Export-PfxCertificate -Cert $server -FilePath .\server.pfx -Password (ConvertTo-SecureString -String "1234" -Force -AsPlainText)

# 3. 產生 client certificate
$client = New-SelfSignedCertificate -DnsName "client1" -CertStoreLocation "Cert:\CurrentUser\My" -Signer $root -KeyExportPolicy Exportable
Export-PfxCertificate -Cert $client -FilePath .\client1.pfx -Password (ConvertTo-SecureString -String "1234" -Force -AsPlainText)

# 匯出 client crt & key
$pfx = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2(".\client1.pfx", "1234")
$pfx.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert) | Set-Content -Encoding Byte -Path client1.crt
