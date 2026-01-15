# 🏥 Calculadora IMC

Uma calculadora de Índice de Massa Corporal (IMC) moderna e completa desenvolvida em C# com interface de terminal interativa usando Spectre.Console.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![License](https://img.shields.io/badge/license-MIT-green)

## ✨ Funcionalidades

### 📊 Cálculo de IMC
- **IMC para Adultos**: Classificação padrão OMS (Magreza, Normal, Sobrepeso, Obesidade I/II/III)
- **Percentil para Crianças/Adolescentes**: Sistema de percentis OMS para idades 2-18 anos usando método LMS
- **Visualização Interativa**: Barra de progresso colorida e sistema de classificação com estrelas
- **Múltiplos Sistemas de Unidades**: Suporte para sistema métrico (kg/m) e imperial (lbs/ft)

### 👤 Gestão de Utilizadores
- **Perfis Personalizados**: Criar e guardar múltiplos utilizadores
- **Dados Completos**: Nome, data de nascimento, sexo, altura, peso, objetivo e nível de atividade
- **Persistência de Dados**: Armazenamento local em JSON (AppData)
- **Histórico de Progresso**: Acompanhamento do peso inicial vs. atual vs. desejado

### 🎨 Temas Visuais
20 temas pré-definidos com esquemas de cores únicos:
- **Default** - Esquema clássico equilibrado
- **Oceano** - Tons de azul e turquesa
- **Floresta** - Verdes naturais
- **Noturno** - Roxos e magentas
- **Pôr do Sol** - Laranjas e vermelhos quentes
- **Neon** - Cores vibrantes e chamativas
- **Matrix** - Verde terminal clássico
- **Drácula** - Inspirado no tema popular
- **Fogo** - Vermelhos intensos
- **Gelo** - Azuis frios e ciano
- **Outono** - Dourados e laranjas
- **Cereja** - Rosas e magentas
- **Cibernético** - Azuis tech
- **Arco-íris** - Todas as cores
- **Vintage** - Tons terrosos
- **Lavanda** - Roxos suaves
- **Deserto** - Amarelos e dourados
- **Menta** - Verdes aquáticos
- **Vampiro** - Vermelho e cinza
- **Tropical** - Cores vibrantes de praia

### 🍎 Conselhos Nutricionais
- **Cálculo de TMB**: Taxa Metabólica Basal usando fórmula Mifflin-St Jeor
- **TDEE**: Total Daily Energy Expenditure baseado no nível de atividade
- **Macros Personalizados**: Proteína, carboidratos e gorduras ajustados ao objetivo
- **Objetivos Suportados**:
  - Perder Peso (défice de 500 kcal)
  - Manter Peso (manutenção)
  - Ganhar Massa (superavit 200-300 kcal)
  - Definição (défice moderado 300 kcal)
  - Recomposição Corporal (défice ligeiro 100 kcal)

### 💡 Sistema de Dicas
- 24+ dicas sobre saúde, nutrição e bem-estar
- Exibição aleatória em cada consulta
- Conselhos práticos e motivacionais

## 🚀 Instalação

### Requisitos
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) ou superior
- Windows, macOS ou Linux

### Download
Baixe a última versão na [página de releases](https://github.com/seu-usuario/CalculadoraIMC/releases).

### Compilar do Código-Fonte
```bash
git clone https://github.com/seu-usuario/CalculadoraIMC.git
cd CalculadoraIMC
dotnet build
dotnet run
```

## 📖 Como Usar

### Menu Principal
```
1) Definir dados     - Editar peso e altura
2) Obter IMC         - Ver cálculo rápido com estrelas
3) Status IMC        - Dashboard completo com todas as informações
7) Definições        - Mudar tema visual
8) Selecionar Utilizador - Trocar ou criar utilizador
9) Sair
```

### Criar Utilizador
1. Escolha "Criar Utilizador" no menu de login
2. Preencha os dados solicitados:
   - Nome (máx. 20 caracteres)
   - Data de nascimento (formato DD/MM/AAAA)
   - Sexo (Masculino/Feminino)
   - Sistema de unidades (Métrico/Imperial)
   - Altura
   - Peso inicial
   - Peso desejado
   - Nível de atividade física
   - Objetivo (perda, ganho, manutenção, etc.)

### Ajustar Peso/Altura
Use as setas do teclado:
- **↑/↓**: Ajuste grande (1kg ou 10cm / 1lb ou 1 inch)
- **←/→**: Ajuste pequeno (100g ou 1cm / 0.5lb)
- **Enter**: Confirmar

### Trocar Tema
1. Acesse "Definições" (opção 7)
2. Use **←/→** para navegar entre temas
3. Pressione **Enter** para confirmar

## 🏗️ Estrutura do Projeto

```
CalculadoraIMC/
├── Core/
│   ├── CalcIMC.cs              # Cálculo e classificação de IMC
│   ├── PercentilIMC.cs         # Sistema de percentis OMS (método LMS)
│   ├── ConselhosNutri.cs       # Cálculos nutricionais (TMB, TDEE, macros)
│   ├── UnitConverter.cs        # Conversão métrico ↔ imperial
│   ├── Constantes.cs           # Constantes do sistema
│   ├── Dicas.cs                # Sistema de dicas aleatórias
│   ├── DateInput.cs            # Input de data com validação
│   └── UserDataManager.cs      # Persistência de dados
├── Menus/
│   ├── MenuPrincipal.cs        # Menu principal
│   ├── MenuLogin.cs            # Login e seleção de utilizador
│   ├── MenuDefinirDados.cs     # Edição de peso/altura
│   ├── MenuObterIMC.cs         # Visualização rápida do IMC
│   ├── MenuStatusIMC.cs        # Dashboard completo
│   └── MenuTemas.cs            # Seleção de temas
├── UI/
│   ├── HelpersUI.cs            # Funções auxiliares de UI
│   └── Temas.cs                # Sistema de temas e cores
├── UserManager/
│   ├── StepResult.cs
│   ├── UserCreationWizard.cs   # Wizard de criação de utilizador
│   └── UserSelector.cs         # Seleção de utilizador
└── Program.cs                  # Ponto de entrada
```

## 🎯 Classificações de IMC

### Adultos (≥18 anos)
| IMC | Classificação |
|-----|---------------|
| < 18,5 | Magreza |
| 18,5 – 24,9 | Normal |
| 25,0 – 29,9 | Sobrepeso |
| 30,0 – 34,9 | Obesidade I |
| 35,0 – 39,9 | Obesidade II |
| ≥ 40,0 | Obesidade III |

### Crianças/Adolescentes (2-18 anos)
Usa percentis OMS com método LMS:
- **< P3**: Magreza
- **P3 – P15**: Abaixo do peso
- **P15 – P85**: Normal
- **P85 – P97**: Sobrepeso
- **≥ P97**: Obesidade

## 🔧 Tecnologias Utilizadas

- **[.NET 10.0](https://dotnet.microsoft.com/)** - Framework principal
- **[Spectre.Console](https://spectreconsole.net/)** - Interface de terminal rica
- **System.Text.Json** - Serialização de dados
- **HttpClient** - Download de recursos

## 📊 Método LMS (Lambda-Mu-Sigma)

O cálculo de percentis para crianças/adolescentes usa o método LMS da OMS, que modela a distribuição do IMC considerando:
- **L (Lambda)**: Transformação Box-Cox para normalizar a distribuição
- **M (Mu)**: Mediana do IMC para cada idade/sexo
- **S (Sigma)**: Coeficiente de variação

Fórmula: `Z = (IMC/M)^L - 1) / (L * S)`

## 📝 Armazenamento de Dados

Os dados são guardados em:
- **Windows**: `%AppData%\CalculadoraIMC\`
- **macOS/Linux**: `~/.config/CalculadoraIMC/`

Ficheiros:
- `users.json` - Todos os utilizadores
- `current_user.txt` - Utilizador atual

## 🤝 Contribuir

Contribuições são bem-vindas! Para contribuir:

1. Faça fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 🙏 Agradecimentos

- [Organização Mundial da Saúde (OMS)](https://www.who.int/) - Tabelas de referência e método LMS
- [Spectre.Console](https://spectreconsole.net/) - Framework de UI incrível
- Comunidade .NET

## 📧 Contato

Para dúvidas ou sugestões, abra uma [issue](https://github.com/seu-usuario/CalculadoraIMC/issues).

---

Feito com ❤️ em C#
