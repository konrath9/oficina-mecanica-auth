variable "aws_region" {
  description = "Regiao AWS (fixa em us-east-1 no AWS Academy Learner Lab)"
  type        = string
  default     = "us-east-1"
}

variable "project_name" {
  description = "Prefixo usado no nome/tags dos recursos, e chave usada para localizar a VPC/RDS do repositorio oficina-mecanica-infra-db via data source"
  type        = string
  default     = "oficina-mecanica"
}

variable "db_username" {
  description = "Usuario master do RDS (mesmo valor configurado no repositorio oficina-mecanica-infra-db)"
  type        = string
  sensitive   = true
}

variable "db_password" {
  description = "Senha master do RDS (mesmo valor configurado no repositorio oficina-mecanica-infra-db)"
  type        = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "Mesma SecretKey configurada no appsettings.json da API principal - garante que o token emitido aqui seja validado la sem nenhuma mudanca"
  type        = string
  sensitive   = true
}

variable "app_public_ip" {
  description = "IP publico (Elastic IP) da EC2 do k3s (repositorio oficina-mecanica-infra-k8s) - destino do proxy HTTP das rotas que nao sao de login"
  type        = string
}

variable "lambda_zip_path" {
  description = "Caminho do pacote .zip do Lambda, gerado no CI antes do terraform apply"
  type        = string
  default     = "../lambda.zip"
}
