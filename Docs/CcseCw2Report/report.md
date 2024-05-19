---
title: "Pacific Tours CCSE-CW2 Report"
author: "2242090"
bibliography: Docs/CcseCw2Report/references.bib
toc: true
toc-title: Table of Contents
toc-depth: 6
csl: Docs/Shared/harvard-imperial-college-london.csl
---

---

> GitHub repository: [https://github.com/iArcanic/pacific-tours-ccse-cw1](https://github.com/iArcanic/pacific-tours-ccse-cw1)

# 1 Executive summary

<!-- 150 words maximum -->

This report presents the results from the security vulnerability testing as well as the implementation of a CI/CD pipeline for the ASP.NET C# application developed for the Pacific Tours hotel company.

The report is split into two sections.

The first section, [Section A](#2-section-a-software-security-vulnerabilities), showcases the results of Static Application Security Testing (SAST) using Synk [@snyk2024] and Dynamic Application Security Testing (DAST) using OWASP ZAP [@owaspzap2024]. The identified vulnerabilities are described in this report along with the steps recommended to remediate them. If a lack of vulnerabilities is found, then potential vulnerabilities with their mitigations are explored and mapped to OWASP guidelines [@owasptop102021].

The second section, [Section B](#3-section-b-software-automation), covers the practical implementation of the CI/CD pipeline using GitHub's CI/CD platform, GitHub actions [@github2024] as a way to automate the build, testing, and deployment process. The pipeline will also include the aforementioned SAST and DAST tools used in [Section A](#2-section-a-software-security-vulnerabilities) in alignment with DevSecOps practices [@paloalto2024]. Stages such as Docker build, image creation, and deployment to a cloud platform are also detailed. Furthermore, it also compares and contrasts any additional vulnerabilities

# 2 Section A: Software security vulnerabilities

<!-- 1450 words maximum -->

## 2.1 Introduction

<!-- 100 words maximum -->

### 2.1.1 Purpose

<!-- 50 words maximum -->

The purpose of this section is to identify any security vulnerabilities in the codebase of the ASP.NET C# project through both static and dynamic analysis. Through this, secure development practices as recommended by OWASP [@owasp2024] are considered. This can help in mitigating potential security risks in the codebase.

### 2.1.2 Tools used

<!-- 50 words maximum -->

The SAST scanning was performed using Synk [@snyk2024] and the DAST analysis was carried out by OWASP ZAP [@owaspzap2024]. These industry-grade tools allow for detailed vulnerability auditing whilst adhering to security practices defined by the National Institute of Standards and Technology (NIST) [@nist2024].

## 2.2 Static Application Security Testing (SAST)

<!-- 400 words maximum -->

### 2.2.1 Methodology

<!-- 100 words maximum -->

SAST analysis involves examining the source code of the ASP.NET C# project to identify any potential security issues without actually having to execute the application. The SAST tool operates based on OWASP's Source Code Review Guide [@owasp2017]. The tool also leverages techniques such as data flow analysis, taint analysis, pattern matching with injection flaw issues, insecure deserialisation, and faulty access control mechanisms [@snyk2024]. The results were then reviewed, and the relevant information will be documented below.

### 2.2.2 Vulnerabilities identified

<!-- 500 words maximum -->

#### 2.2.2.1 NuGet.Packaging – Improper access control

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.VisualStudio.Web.CodeGeneration.Design@7.0.11`
> - `Microsoft.DotNet.Scaffolding.Shared@7.0.11`
> - `NuGet.ProjectModel@6.6.1`
> - `NuGet.DependencyResolver.Core@6.6.1`
> - `NuGet.Protocol@6.6.1`
> - `NuGet.Packaging@6.6.1`
>
> Severity: CRITICAL
>
> Priority score: 562

NuGet.Packaging [@nuget2024] is an implementation by Nuget, specifically for reading `nupkg` and `nuspec` package specification files.

The version of NuGet.Packaging being used, `6.6.1`, is vulnerable to Improper Access Control when using `X.509` chain building APIs. However, due to a logic flaw, the `X.509` certificate cannot be completely validated. An attacker could exploit this by using an untrusted certificate with corrupted signatures, triggering the bug in the framework. The framework will therefore correctly report that the `X.509` chain building failed as expected but will return an incorrect error message for this failure. Any application that relies on this error checking logic may accidentally treat this situation as a successful build. To extend this, an attacker could use this to bypass an application's typical authentication and authorisation logic.

#### 2.2.2.2 Anti-forgery token validation disabled

<!-- 50 words maximum -->

> Introduced through:
>
> - `ErrorModel` class
>
> Severity: LOW
>
> Priority score: 450

#### 2.2.2.3 Azure.Identity – Remote Code Execution (RCE)

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
> - `Azure.Identity@1.7.0`
>
> Severity: HIGH
>
> Priority score: 440

#### 2.2.2.4 Microsoft.Data.SqlClient – Unprotected storage of credentials

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
>
> Severity: HIGH
>
> Priority score: 375

#### 2.2.2.5 System.Net.Http – Denail of Service (DoS)

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.VisualStudio.Web.CodeGeneration.Design@7.0.11`
> - `Microsoft.DotNet.Scaffolding.Shared@7.0.11`
> - `Microsoft.CodeAnalysis.CSharp.Features@4.4.0`
> - `Microsoft.CodeAnalysis.Features@4.4.0`
> - `Microsoft.DiaSymReader@1.4.0`
> - `NETStandard.Library@1.6.1`
> - `System.Net.Http@4.3.0`
>
> Severity: HIGH
>
> Priority score: 375

#### 2.2.2.6 System.Net.Http – Improper certificate validation

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.VisualStudio.Web.CodeGeneration.Design@7.0.11`
> - `Microsoft.DotNet.Scaffolding.Shared@7.0.11`
> - `Microsoft.CodeAnalysis.CSharp.Features@4.4.0`
> - `Microsoft.CodeAnalysis.Features@4.4.0`
> - `Microsoft.DiaSymReader@1.4.0`
> - `NETStandard.Library@1.6.1`
> - `System.Net.Http@4.3.0`
>
> Severity: HIGH
>
> Priority score: 375

#### 2.2.2.7 System.Net.Http – Information exposure

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.VisualStudio.Web.CodeGeneration.Design@7.0.11`
> - `Microsoft.DotNet.Scaffolding.Shared@7.0.11`
> - `Microsoft.CodeAnalysis.CSharp.Features@4.4.0`
> - `Microsoft.CodeAnalysis.Features@4.4.0`
> - `Microsoft.DiaSymReader@1.4.0`
> - `NETStandard.Library@1.6.1`
> - `System.Net.Http@4.3.0`
>
> Severity: HIGH
>
> Priority score: 375

#### 2.2.2.8 System.Text.RegularExpressions – Regular Expression Denial of Service (ReDoS)

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.VisualStudio.Web.CodeGeneration.Design@7.0.11`
> - `Microsoft.DotNet.Scaffolding.Shared@7.0.11`
> - `Microsoft.CodeAnalysis.CSharp.Features@4.4.0`
> - `Microsoft.CodeAnalysis.Features@4.4.0`
> - `Microsoft.DiaSymReader@1.4.0`
> - `NETStandard.Library@1.6.1`
> - `System.Net.Http@4.3.0`
> - `System.Text.RegularExpressions@4.3.0`
>
> Severity: HIGH
>
> Priority score: 375

#### 2.2.2.9 System.Net.Http – Privilege escalation

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.VisualStudio.Web.CodeGeneration.Design@7.0.11`
> - `Microsoft.DotNet.Scaffolding.Shared@7.0.11`
> - `Microsoft.CodeAnalysis.CSharp.Features@4.4.0`
> - `Microsoft.CodeAnalysis.Features@4.4.0`
> - `Microsoft.DiaSymReader@1.4.0`
> - `NETStandard.Library@1.6.1`
> - `System.Net.Http@4.3.0`
>
> Severity: HIGH
>
> Priority score: 365

#### 2.2.2.10 Microsoft.IdentityModel.JsonWebTokens – Resource exhaustion

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
> - `Microsoft.IdentityModel.JsonWebTokens@6.24.0`
>
> Severity: MEDIUM
>
> Priority score: 340

#### 2.2.2.11 System.IdentityModel.Tokens.Jwt – Resource exhaustion

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
> - `Microsoft.IdentityModel.Protocols.OpenIdConnect@6.24.0`
> - `System.IdentityModel.Tokens.Jwt@6.24.0`
>
> Severity: MEDIUM
>
> Priority score: 340

#### 2.2.2.12 Azure.Identity – Information exposure through an error message

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
> - `Azure.Identity@1.7.0`
>
> Severity: MEDIUM
>
> Priority score: 275

#### 2.2.2.13 System.Net.Http – Authentication bypass

<!-- 50 words maximum -->

> Introduced through:
>
> - `Microsoft.VisualStudio.Web.CodeGeneration.Design@7.0.11`
> - `Microsoft.DotNet.Scaffolding.Shared@7.0.11`
> - `Microsoft.CodeAnalysis.CSharp.Features@4.4.0`
> - `Microsoft.CodeAnalysis.Features@4.4.0`
> - `Microsoft.DiaSymReader@1.4.0`
> - `NETStandard.Library@1.6.1`
> - `System.Net.Http@4.3.0`
>
> Severity: MEDIUM
>
> Priority score: 265

## 2.3 Dynamic Application Security Testing (DAST)

<!-- 500 words maximum -->

### 2.3.1 Methodology

<!-- 100 words maximum -->

### 2.3.2 Vulnerabilities identified

#### 2.3.2.1 Content Security Policy (CSP) header not set

#### 2.3.2.2 Proxy disclosure

#### 2.3.2.3 Application error disclosure

#### 2.3.2.4 Permissions policy header not set

#### 2.3.2.5 Strict-transport-security header not set

#### 2.3.2.6 Non-storable content

#### 2.3.2.7 Storable and cachable content

#### 2.3.2.8 User-agent fuzzer

# 3 Section B: Software automation

<!-- 950 words maximum -->

## 3.1 Introduction

<!-- 100 words maximum -->

### 3.1.1 Purpose

<!-- 50 words maximum -->

### 3.1.2 Tools and technologies used

<!-- 50 words maximum -->

## 3.2 CI/CD pipeline implementation

### 3.2.1 `test`

```yaml
test:
  runs-on: windows-latest

  steps:
    - name: Checkout source code
      uses: actions/checkout@v4

    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_CORE_VERSION }}

    - name: Restore project dependencies
      run: dotnet restore ${{ env.SOLUTION_NAME }}

    - name: Test project
      run: dotnet test --no-restore --verbosity normal
```

### 3.2.2 `docker-build-and-push-to-gar`

```yaml
docker-build-and-push-to-gar:
  runs-on: ubuntu-latest
  needs: [sast, test]

  steps:
    - name: Checkout source code
      uses: actions/checkout@v4

    - name: Google authentication
      uses: google-github-actions/auth@v2
      with:
        credentials_json: ${{ env.GCP_SA_KEY }}

    - name: Setup Google Cloud SDK
      uses: google-github-actions/setup-gcloud@v1
      with:
        version: "latest"
        project_id: ${{ env.GCP_PROJECT_ID }}

    - name: Build Docker Image
      run: |
        echo "Building Docker image..."
        docker build -t ${{ env.GCP_REGION }}-docker.pkg.dev/${{ env.GCP_PROJECT_ID }}/${{ env.GCP_GAR_REPO }}/${{ env.CONTAINER_IMAGE_NAME }}:${{ github.sha }} .

    - name: Configure Docker client
      run: |
        gcloud auth configure-docker --quiet
        gcloud auth configure-docker ${{ env.GCP_REGION }}-docker.pkg.dev --quiet

    - name: Push Docker image to Google Artifact Registry
      run: |
        echo "Pushing Docker image to Google Artifact Registry..."
        docker push ${{ env.GCP_REGION }}-docker.pkg.dev/${{ env.GCP_PROJECT_ID }}/${{ env.GCP_GAR_REPO }}/${{ env.CONTAINER_IMAGE_NAME }}:${{ github.sha }}
```

### 3.2.3 `deploy`

```yaml
deploy:
  runs-on: ubuntu-latest
  needs: docker-build-and-push-to-gar

  steps:
    - name: Checkout source code
      uses: actions/checkout@v4

    - name: Google authentication
      uses: google-github-actions/auth@v2
      with:
        credentials_json: ${{ env.GCP_SA_KEY }}

    - name: Setup Google Cloud SDK
      uses: google-github-actions/setup-gcloud@v1
      with:
        version: "latest"
        project_id: ${{ env.GCP_PROJECT_ID }}

    - name: Deploy to Google Cloud Run
      run: |
        gcloud run deploy ${{ env.GCP_CLOUD_RUN_SERVICE }} \
          --image=${{ env.GCP_REGION }}-docker.pkg.dev/${{ env.GCP_PROJECT_ID }}/${{ env.GCP_GAR_REPO }}/${{ env.CONTAINER_IMAGE_NAME }}:${{ github.sha }} \
          --region=${{ env.GCP_REGION }} \
          --allow-unauthenticated \
          --memory=1Gi \
          --cpu=1 \
          --min-instances=0 \
          --max-instances=2 \
          --concurrency=80 \
          --add-cloudsql-instances=${{ env.GCP_CLOUD_SQL_INSTANCE }} \
          --update-env-vars ASPNETCORE_ENVIRONMENT=Development \
          --platform=managed
```

### 3.2.4 `database-migration`

```yaml
database-migration:
  runs-on: ubuntu-latest
  needs: deploy

  steps:
    - name: Checkout source code
      uses: actions/checkout@v4

    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_CORE_VERSION }}

    - name: Install Dotnet Entity Framework CLI tool
      run: |
        dotnet tool install --global dotnet-ef
        dotnet tool restore

    - name: Download Cloud SQL Auth Proxy
      run: |
        curl -o cloud-sql-proxy https://storage.googleapis.com/cloud-sql-connectors/cloud-sql-proxy/v2.11.0/cloud-sql-proxy.linux.amd64

    - name: Make the Cloud SQL Auth Proxy executable
      run: chmod +x cloud-sql-proxy

    - name: Google authentication
      uses: google-github-actions/auth@v2
      with:
        credentials_json: ${{ env.GCP_SA_KEY }}

    - name: Setup Google Cloud SDK
      uses: google-github-actions/setup-gcloud@v1
      with:
        version: "latest"
        project_id: ${{ env.GCP_PROJECT_ID }}

    - name: Start Cloud SQL Auth Proxy
      run: |
        ./cloud-sql-proxy ${{ env.GCP_CLOUD_SQL_INSTANCE }} &

    - name: Run database migrations
      run: |
        dotnet ef database update
      env:
        ASPNETCORE_ENVIRONMENT: Development
        ConnectionStrings__DefaultConnection: ${{ env.CONNECTION_STRING_GITHUB_ACTIONS }}

    - name: Stop Cloud SQL Proxy
      run: killall cloud-sql-proxy
```

## 3.3 Security testing in CI/CD pipeline

### 3.3.1 `sast`

```yaml
sast:
  runs-on: ubuntu-latest
  needs: test
  permissions: write-all

  steps:
    - name: Checkout source code
      uses: actions/checkout@v4

    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_CORE_VERSION }}

    - name: Restore project dependencies
      run: dotnet restore ${{ env.SOLUTION_NAME }}

    - name: Run Snyk to check for vulnerabilities
      uses: snyk/actions/dotnet@master
      continue-on-error: true
      with:
        args: --sarif-file-output=snyk.sarif

    - name: Upload result to GitHub code scanning
      uses: github/codeql-action/upload-sarif@v3
      with:
        sarif_file: snyk.sarif
        category: snyk-sast-analysis
```

### 3.3.2 `dast`

```yaml
dast:
  runs-on: ubuntu-latest
  needs: deploy

  steps:
    - name: Checkout source code
      uses: actions/checkout@v4

    - name: Google authentication
      uses: google-github-actions/auth@v2
      with:
        credentials_json: ${{ env.GCP_SA_KEY }}

    - name: Setup Google Cloud SDK
      uses: google-github-actions/setup-gcloud@v1
      with:
        version: "latest"
        project_id: ${{ env.GCP_PROJECT_ID }}

    - name: Get Cloud Run Service URL
      id: get-url
      run: |
        URL=$(gcloud run services describe ${{ env.GCP_CLOUD_RUN_SERVICE }} --region=${{ env.GCP_REGION }} --format 'value(status.url)')
        echo "::set-output name=url::$URL"

    - name: Run OWASP ZAP scan
      uses: zaproxy/action-full-scan@v0.10.0
      with:
        target: ${{ steps.get-url.outputs.url }}
```

## 3.4 Comparison with [Section A](#2-section-a-software-security-vulnerabilities)

### 3.4.1 Static Application Security Testing (SAST)

No additional vulnerabilities (other than those documented in [Section A, 2.2.2](#222-vulnerabilities-identified)) were detected during the CI/CD pipeline `sast` job.

### 3.4.2 Dynamic Application Security Testing (DAST)

#### 3.4.2.2 Additional vulnerabilities identified

##### 3.4.2.1.1 Missing anti-clickjacking header

##### 3.4.2.1.2 Vulnerable JavaScript library

##### 3.4.2.1.3 Cookie without secure flag

##### 3.4.2.1.4 Dangerous JavaScript functions

##### 3.4.2.1.5 X-content-type-options header missing

##### 3.4.2.1.6 Authentication request identified

##### 3.4.2.1.7 Cookie slack detector

##### 3.4.2.1.8 Information disclosure – suspicious comments

##### 3.4.2.1.9 Re-examine cache-control directives

##### 3.4.2.1.10 Session management response identified

##### 3.4.2.1.11 User controllable HTML element attribute (potential XSS)

# 4 Appendices

# 5 References
