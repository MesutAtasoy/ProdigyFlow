# ProdigyFlow AI

**Enhancing CI/CD Pipelines with AI using .NET & Semantic Kernel**

ProdigyFlow AI is a .NET-based tool designed to make your CI/CD pipelines smarter using AI. It leverages **Semantic Kernel** and large language models (LLMs) to:

* Prioritize tests based on code changes.
* Score PRs for risk before merging.
* Provide natural language insights directly in PRs.

---

## 🔹 Features

### 1. Intelligent Test Prioritization

* Predicts which tests are most likely to fail based on the PR diff.
* Runs only relevant tests to save time and CI resources.

### 2. Risk Scoring

* Analyzes PRs to provide a risk score (0-100).
* Returns confidence and detailed reasons for the risk.
* Helps teams make risk-aware decisions before merging.

### 3. GitHub Actions Integration

* Fully integrates with GitHub Actions pipelines.
* Automatically comments on PRs with AI insights.
* Supports adaptive CI/CD workflows.

---

## ⚙️ Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* GitHub repository with Actions enabled
* API key for your chosen LLM provider (e.g., Azure OpenAI, Google Gemini)
* xUnit tests for your .NET projects

---

## 🏗️ Setup & Installation

1. Clone the repository:

```bash
git clone https://github.com/your-org/ProdigyFlow.AI.git
cd ProdigyFlow.AI
```

2. Restore dependencies:

```bash
dotnet restore ProdigyFlow.sln
```

3. Build the solution:

```bash
dotnet build ProdigyFlow.sln --configuration Release
```

4. Configure environment variables for your LLM provider:

```bash
export GOOGLE_GEMINI_API_KEY="your_api_key_here"
```

---

## 🏃 Running the AI Services

### Test Prioritization

```bash
dotnet run --project src/ProdigyFlow.AI TestPrioritization pr_diff.txt
```

This will:

* Summarize the PR diff.
* Generate a prioritized list of tests (`prioritized_tests.txt`).
* Save risk assessment to `ai_risk.txt`.


## 📦 GitHub Actions Integration

The project includes a sample workflow `.github/workflows/prodigyflow-ci.yml`:

* Checkout code
* Restore and build
* Generate PR diff
* Run AI Test Prioritization
* Execute prioritized tests
* Run Anomaly Detection
* Post insights as PR comment
* Add AI review label

> The workflow adapts to PR risk and test results automatically.

---

## 📝 Example Outputs

### Prioritized Tests

```
UserServiceTests.Login_Should_Throw_When_UserNotFound
UserProfileTests.Update_Should_HandleNullValues
```

### Risk Score

```
RiskScore: 65
Confidence: 75
Reasons:
- Modifies core data models.
- Introduces new dependencies.
- Lacks adequate unit tests.
```



---

## 💡 Extending ProdigyFlow AI

* Add custom prompts for more detailed insights.
* Integrate with Slack/Teams bots for natural language queries.
* Combine with ML.NET for predictive build duration or failure modeling.
* Enhance risk scoring with historical PR analytics.

---

## 🏷️ License

This project is licensed under the MIT License. See `LICENSE` for details.

---

## 📢 About

ProdigyFlow AI is designed to make DevOps smarter by combining AI with practical .NET CI/CD workflows. Perfect for developers, DevOps engineers, and architects looking to improve pipeline efficiency and reliability.
