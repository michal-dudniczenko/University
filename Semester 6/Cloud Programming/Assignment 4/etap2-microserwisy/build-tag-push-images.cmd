chdir C:\Users\Michal\Documents\Studia\Semester 6\Cloud Programming\Assignment 2\ECommerceSystem

docker build -f Dockerfile_UserService -t user-service .
docker build -f Dockerfile_ProductService -t product-service .
docker build -f Dockerfile_CartService -t cart-service .
docker build -f Dockerfile_OrderService -t order-service .
docker build -f Dockerfile_NotificationService -t notification-service .

aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 734054263645.dkr.ecr.us-east-1.amazonaws.com

docker tag user-service:latest 734054263645.dkr.ecr.us-east-1.amazonaws.com/user-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/user-service:latest

docker tag product-service:latest 734054263645.dkr.ecr.us-east-1.amazonaws.com/product-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/product-service:latest

docker tag cart-service:latest 734054263645.dkr.ecr.us-east-1.amazonaws.com/cart-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/cart-service:latest

docker tag order-service:latest 734054263645.dkr.ecr.us-east-1.amazonaws.com/order-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/order-service:latest

docker tag notification-service:latest 734054263645.dkr.ecr.us-east-1.amazonaws.com/notification-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/notification-service:latest

pause