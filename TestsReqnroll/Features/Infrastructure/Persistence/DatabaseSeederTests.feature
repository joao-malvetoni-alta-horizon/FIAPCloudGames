# language: pt-br
Funcionalidade: Semeadura do Banco de Dados

  Como sistema de persistência
  Eu quero popular o banco de dados com dados iniciais
  Para garantir que exista um administrador padrão no primeiro acesso

  Cenário: Criar administrador padrão quando o banco estiver vazio
    Dado que não existe nenhum usuário administrador no sistema
    Quando eu executar a semeadura do banco de dados
    Então um usuário com e-mail "admin@fcg.com" deve ser criado
    E a senha deve ser criptografada antes de ser salva
    E o usuário deve possuir o cargo de "Administrator"

  Cenário: Não duplicar administrador se ele já existir
    Dado que já existe um administrador com o e-mail "admin@fcg.com" no sistema
    Quando eu executar a semeadura do banco de dados
    Então o sistema não deve criar um novo administrador
    E o processo de criptografia de senha não deve ser acionado