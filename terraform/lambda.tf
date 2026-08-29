# A execution role e a LabRole ja existente na conta - o AWS Academy Learner Lab bloqueia
# iam:CreateRole, entao nenhuma role/policy nova e criada aqui (ver ADR sobre restricoes do
# AWS Academy). O ARN e fixo porque a conta e dedicada a este projeto.
locals {
  lab_role_arn = "arn:aws:iam::159157616728:role/LabRole"
}

resource "aws_lambda_function" "auth" {
  function_name = "${var.project_name}-auth"
  role          = local.lab_role_arn
  handler       = "OficinaMecanica.Auth"
  runtime       = "dotnet8"
  timeout       = 15
  memory_size   = 256

  filename         = var.lambda_zip_path
  source_code_hash = filebase64sha256(var.lambda_zip_path)

  vpc_config {
    subnet_ids         = data.aws_subnets.private.ids
    security_group_ids = [aws_security_group.lambda.id]
  }

  environment {
    variables = {
      DB_CONNECTION_STRING = "Host=${data.aws_db_instance.main.address};Port=${data.aws_db_instance.main.port};Database=${data.aws_db_instance.main.db_name};Username=${var.db_username};Password=${var.db_password};SSL Mode=Require;Trust Server Certificate=true"
      JWT_SECRET_KEY       = var.jwt_secret_key
      JWT_ISSUER           = "OficinaMecanicaAPI"
      JWT_AUDIENCE         = "OficinaMecanicaClients"
    }
  }

  tags = {
    Name    = "${var.project_name}-auth"
    Project = var.project_name
  }
}

resource "aws_lambda_permission" "apigw" {
  statement_id  = "AllowAPIGatewayInvoke"
  action        = "lambda:InvokeFunction"
  function_name = aws_lambda_function.auth.function_name
  principal     = "apigateway.amazonaws.com"
  source_arn    = "${aws_apigatewayv2_api.main.execution_arn}/*/*"
}
