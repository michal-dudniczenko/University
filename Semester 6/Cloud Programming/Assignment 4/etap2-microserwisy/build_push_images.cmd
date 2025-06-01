chdir C:\Users\Michal\Documents\Studia\Semester 6\Cloud Programming\Assignment 2\ECommerceSystem

docker build -f Dockerfile_UserService -t 734054263645.dkr.ecr.us-east-1.amazonaws.com/user-service:latest .
docker build -f Dockerfile_ProductService -t 734054263645.dkr.ecr.us-east-1.amazonaws.com/product-service:latest .
docker build -f Dockerfile_CartService -t 734054263645.dkr.ecr.us-east-1.amazonaws.com/cart-service:latest .
docker build -f Dockerfile_OrderService -t 734054263645.dkr.ecr.us-east-1.amazonaws.com/order-service:latest .
docker build -f Dockerfile_NotificationService -t 734054263645.dkr.ecr.us-east-1.amazonaws.com/notification-service:latest .

aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 734054263645.dkr.ecr.us-east-1.amazonaws.com

docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/user-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/product-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/cart-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/order-service:latest
docker push 734054263645.dkr.ecr.us-east-1.amazonaws.com/notification-service:latest

pause