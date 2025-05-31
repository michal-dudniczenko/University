provider "aws" {
  region = "us-east-1"
}

data "aws_vpc" "default" {
  default = true
}

data "aws_subnet" "ecommerce-subnet" {
  filter {
    name   = "tag:Name"
    values = ["ecommerce-subnet"]
  }
}

resource "aws_security_group" "ecommerceSg" {
  name        = "ecommerceSg"
  description = "Allow SSH, 5001-5005"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "Ecommerce Service"
    from_port   = 5001
    to_port     = 5005
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

data "aws_iam_instance_profile" "lab_profile" {
  name = "LabInstanceProfile"
}

resource "aws_instance" "ecommerceSystem" {
  ami                  = "ami-084568db4383264d4"
  instance_type        = "t2.micro"
  key_name             = "aws key"
  subnet_id            = data.aws_subnet.ecommerce-subnet.id
  iam_instance_profile = data.aws_iam_instance_profile.lab_profile.name

  vpc_security_group_ids = [aws_security_group.ecommerceSg.id]

  user_data = <<-EOF
                #!/bin/bash
                sudo apt update
                sudo apt install docker.io -y
                sudo systemctl start docker
                sudo systemctl enable docker
                sudo snap install aws-cli --classic
                aws ecr get-login-password --region us-east-1 | sudo docker login --username AWS --password-stdin 734054263645.dkr.ecr.us-east-1.amazonaws.com
                sudo docker pull 734054263645.dkr.ecr.us-east-1.amazonaws.com/user-service:latest
                sudo docker pull 734054263645.dkr.ecr.us-east-1.amazonaws.com/product-service:latest
                sudo docker pull 734054263645.dkr.ecr.us-east-1.amazonaws.com/cart-service:latest
                sudo docker pull 734054263645.dkr.ecr.us-east-1.amazonaws.com/order-service:latest
                sudo docker pull 734054263645.dkr.ecr.us-east-1.amazonaws.com/notification-service:latest
                sudo docker network create ecommerce-network
                sudo docker run -d -p 5001:5001 --network ecommerce-network --name user-service 734054263645.dkr.ecr.us-east-1.amazonaws.com/user-service:latest
                sudo docker run -d -p 5002:5002 --network ecommerce-network --name product-service 734054263645.dkr.ecr.us-east-1.amazonaws.com/product-service:latest
                sudo docker run -d -p 5003:5003 --network ecommerce-network --name cart-service 734054263645.dkr.ecr.us-east-1.amazonaws.com/cart-service:latest
                sudo docker run -d -p 5004:5004 --network ecommerce-network --name order-service 734054263645.dkr.ecr.us-east-1.amazonaws.com/order-service:latest
                sudo docker run -d -p 5005:5005 --network ecommerce-network --name notification-service 734054263645.dkr.ecr.us-east-1.amazonaws.com/notification-service:latest
                EOF

  tags = {
    Name = "ecommerce-system"
  }
}