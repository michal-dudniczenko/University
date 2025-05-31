provider "aws" {
  region = "us-east-1"
}

data "aws_vpc" "default" {
  default = true
}

resource "aws_subnet" "ecommerce-subnet" {
  vpc_id                  = data.aws_vpc.default.id
  cidr_block              = "172.31.1.0/24"
  availability_zone       = "us-east-1a"
  map_public_ip_on_launch = true

  tags = {
    Name = "ecommerce-subnet"
  }
}

resource "aws_subnet" "ecommerce-subnet2" {
  vpc_id                  = data.aws_vpc.default.id
  cidr_block              = "172.31.2.0/24"
  availability_zone       = "us-east-1b"
  map_public_ip_on_launch = true

  tags = {
    Name = "ecommerce-subnet2"
  }
}

resource "aws_db_subnet_group" "db_subnet_group" {
  name       = "db_subnet_group"
  subnet_ids = [
    aws_subnet.ecommerce-subnet.id,
    aws_subnet.ecommerce-subnet2.id,
  ]

  tags = {
    Name = "db_subnet_group"
  }
}

resource "aws_security_group" "messageBrokerSg" {
  name        = "messageBrokerSg"
  description = "Allow SSH, RabbitMQ, and RabbitMQ management traffic"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "RabbitMQ"
    from_port   = 5672
    to_port     = 5672
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    description = "RabbitMQ Management UI"
    from_port   = 15672
    to_port     = 15672
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

resource "aws_security_group" "sqlServerDbSg" {
  name        = "sqlServerDbSg"
  description = "Allow 1433"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    description = "MSSQL"
    from_port   = 1433
    to_port     = 1433
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


resource "aws_instance" "messageBroker" {
  ami           = "ami-084568db4383264d4"
  instance_type = "t2.micro"
  key_name      = "aws key"
  subnet_id     = aws_subnet.ecommerce-subnet.id

  vpc_security_group_ids = [aws_security_group.messageBrokerSg.id]

  user_data = <<-EOF
                #!/bin/bash
                apt-get update
                apt-get install -y rabbitmq-server
                rabbitmq-plugins enable rabbitmq_management
                rabbitmqctl add_user michal michal
                rabbitmqctl set_user_tags michal administrator
                rabbitmqctl set_permissions -p / michal ".*" ".*" ".*"
                systemctl stop rabbitmq-server
                systemctl start rabbitmq-server
                systemctl enable rabbitmq-server
                EOF

  tags = {
    Name = "message-broker"
  }
}

resource "aws_db_instance" "mssqlDb" {
  identifier             = "mssql-db"
  allocated_storage      = 20
  engine                 = "sqlserver-ex"
  engine_version         = "15.00.4430.1.v1"
  instance_class         = "db.t3.micro"
  username               = "admin"
  password               = "zcN4wRhp722ItQ7s"
  skip_final_snapshot    = true
  publicly_accessible    = true
  vpc_security_group_ids = [aws_security_group.sqlServerDbSg.id]
  multi_az               = false
  db_subnet_group_name = aws_db_subnet_group.db_subnet_group.name
}

resource "aws_ecr_repository" "user-service" {
  name                 = "user-service"
  image_tag_mutability = "MUTABLE"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "user-service"
  }
}

resource "aws_ecr_repository" "product-service" {
  name                 = "product-service"
  image_tag_mutability = "MUTABLE"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "product-service"
  }
}

resource "aws_ecr_repository" "cart-service" {
  name                 = "cart-service"
  image_tag_mutability = "MUTABLE"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "cart-service"
  }
}

resource "aws_ecr_repository" "order-service" {
  name                 = "order-service"
  image_tag_mutability = "MUTABLE"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "order-service"
  }
}

resource "aws_ecr_repository" "notification-service" {
  name                 = "notification-service"
  image_tag_mutability = "MUTABLE"
  force_delete = true

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "notification-service"
  }
}