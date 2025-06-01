docker stop user-service
docker stop product-service
docker stop cart-service
docker stop order-service
docker stop notification-service

docker rm user-service
docker rm product-service
docker rm cart-service
docker rm order-service
docker rm notification-service

docker network rm ecommerce-network

pause
