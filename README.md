# create-multiple-selfsigned-certificates

使用 curl 直接測試：
curl -vk https://localhost:5001/api/test \
  --cert client1.crt \
  --key client1.key \
  --cacert rootCA.crt

