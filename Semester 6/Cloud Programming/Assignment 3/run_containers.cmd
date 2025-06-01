docker network create ecommerce-network

docker run -d -p 5001:5001 --network ecommerce-network --name user-service user-service
docker run -d -p 5002:5002 --network ecommerce-network --name product-service product-service
docker run -d -p 5003:5003 --network ecommerce-network --name cart-service cart-service
docker run -d -p 5004:5004 --network ecommerce-network --name order-service order-service
docker run -d -p 5005:5005 --network ecommerce-network --name notification-service notification-service

pause