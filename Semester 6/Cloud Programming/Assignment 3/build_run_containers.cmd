docker build -f Dockerfile_UserService -t userservice .
docker build -f Dockerfile_ProductService -t productservice .
docker build -f Dockerfile_CartService -t cartservice .
docker build -f Dockerfile_OrderService -t orderservice .
docker build -f Dockerfile_NotificationService -t notificationservice .

docker run -d -p 5001:5001 --network ecommerce-network --name userservice-app userservice
docker run -d -p 5002:5002 --network ecommerce-network --name productservice-app productservice
docker run -d -p 5003:5003 --network ecommerce-network --name cartservice-app cartservice
docker run -d -p 5004:5004 --network ecommerce-network --name orderservice-app orderservice
docker run -d -p 5005:5005 --network ecommerce-network --name notificationservice-app notificationservice

pause