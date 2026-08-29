resource "aws_security_group" "lambda" {
  name        = "${var.project_name}-auth-lambda-sg"
  description = "Security group da Lambda de autenticacao (o RDS ja libera 5432 para todo o CIDR da VPC)"
  vpc_id      = data.aws_vpc.shared.id

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name    = "${var.project_name}-auth-lambda-sg"
    Project = var.project_name
  }
}
