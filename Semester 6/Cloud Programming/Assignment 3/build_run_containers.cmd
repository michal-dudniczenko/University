chdir C:\Users\Michal\Documents\Studia\Semester 6\Cloud Programming\Assignment 2\ECommerceSystem

docker build -f Dockerfile_UserService -t user-service .
docker build -f Dockerfile_ProductService -t product-service .
docker build -f Dockerfile_CartService -t cart-service .
docker build -f Dockerfile_OrderService -t order-service .
docker build -f Dockerfile_NotificationService -t notification-service .

docker run -d -p 5001:5001 --network ecommerce-network --name user-service user-service
docker run -d -p 5002:5002 --network ecommerce-network --name product-service product-service
docker run -d -p 5003:5003 --network ecommerce-network --name cart-service cart-service
docker run -d -p 5004:5004 --network ecommerce-network --name order-service order-service
docker run -d -p 5005:5005 --network ecommerce-network --name notification-service notification-service

pause