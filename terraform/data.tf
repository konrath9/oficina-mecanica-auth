# VPC/subnets e RDS vem do repositorio oficina-mecanica-infra-db, localizados por tag/identificador
# (nao por remote state) para manter os repositorios desacoplados - ver ADR 0002.

data "aws_vpc" "shared" {
  filter {
    name   = "tag:Project"
    values = [var.project_name]
  }
}

data "aws_subnets" "private" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.shared.id]
  }

  filter {
    name   = "tag:Tier"
    values = ["private"]
  }
}

data "aws_db_instance" "main" {
  db_instance_identifier = "${var.project_name}-db"
}
