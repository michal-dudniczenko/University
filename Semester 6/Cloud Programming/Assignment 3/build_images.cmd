chdir C:\Users\Michal\Documents\Studia\Semester 6\Cloud Programming\Assignment 2\ECommerceSystem

docker build -f Dockerfile_UserService -t user-service .
docker build -f Dockerfile_ProductService -t product-service .
docker build -f Dockerfile_CartService -t cart-service .
docker build -f Dockerfile_OrderService -t order-service .
docker build -f Dockerfile_NotificationService -t notification-service .

pause