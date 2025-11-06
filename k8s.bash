kubectl create secret tls server-tls \
  --cert=server.crt \
  --key=server.key

kubectl create secret generic server-ca \
  --from-file=ca.crt=ca-chain.crt
