---
title: "Pacific Tours CCSE-CW2 Report"
author: "2242090"
bibliography: Docs/CcseCw2Report/references.bib
toc: true
toc-title: Table of Contents
toc-depth: 4
geometry: "left=1.25cm, right=1.25cm, top=1.25cm, bottom=1.25cm"
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

#### 2.2.2.1 NuGet.Packaging – Improper access control

> CVE-2024-0057: NET, .NET Framework, and Visual Studio Security Feature Bypass Vulnerability [@cve2024]
>
> CWE-284: Improper Access Control [@cwe2024]
>
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

The version of NuGet.Packaging being used, `6.6.1`, is vulnerable to Improper Access Control when using `X.509` chain building APIs. However, due to a logic flaw, the `X.509` certificate cannot be completely validated. An attacker could exploit this by using an untrusted certificate with corrupted signatures, triggering the bug in the framework. The framework will therefore correctly report that the `X.509` chain building failed as expected but will return an incorrect error message for this failure. Any application that relies on this error-checking logic may accidentally treat this situation as a successful build. To extend this, an attacker could use this to bypass an application's typical authentication and authorisation logic.

This vulnerability can be resolved simply by updating or changing the NuGet.Packaging version to the following versions: `5.11.6`, `6.0.6`, `6.3.4`, `6.4.3`, `6.6.2`, `6.7.1`, and `6.8.1`.

#### 2.2.2.2 Anti-forgery token validation disabled

> CWE-352: Cross-Site Request Forgery (CSRF) [@cwe2023]
>
> Introduced through:
>
> - `ErrorModel` class
>
> Severity: LOW
>
> Priority score: 450

Specifically, the `ErrorModel` class below has the `[IgnoreAntiForgeryToken]` annotation set meaning that cross-site request forgeries are more likely to occur.

```csharp
namespace asp_net_core_web_app_authentication_authorisation.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
```

A cross-site request forgery is an attack where a threat actor attempts to take advantage of a user's authorised credentials (via a browser cookie or a relevant token) to impersonate a trusted user and perform unathorised access under a valid guise. In this case, the application's server cannot differentiate between a legitimate or malicious request. This type of attack usually takes place through social engineering, i.e. a link or popup that a user clicks causing an unauthorised request to be sent to the web server.

This vulnerability can be prevented in ASP.NET Model View Controllers (MVCs) by using the `[ValidateAntiForgeryToken]` attribute to a class to change the state of the server.

#### 2.2.2.3 `Azure.Identity` – Remote Code Execution (RCE)

> CVE-2023-36414: Azure Identity SDK Remote Code Execution Vulnerability [@cve2023]
>
> CWE-94: Improper Control of Generation of Code ('Code Injection') [@2cwe2023]
>
> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
> - `Azure.Identity@1.7.0`
>
> Severity: HIGH
>
> Priority score: 440

The `Azure.Identity` library provides token authentication support (via Microsoft Entra ID) for access to the Azure Software Development Kit (SDK). Through a set of provided `TokenCredential` implementations, Azure SDK clients can be built that complement Microsoft Entra token authentication [@microsoftlearn2024].

Any affected versions of the `Azure.Identity` packages are vulnerable to Remote Code Execution (RCE) attacks, more specifically, when a carefully crafted operating system level command is passed to a certain ASP.NET SDK property. This command can then be passed to the underlying Command-Line Interface (CLI).

The vulnerability can be mitigated by simply updating the `Azure.Identity` package to `1.10.2`.

#### 2.2.2.4 `Microsoft.Data.SqlClient` – Unprotected storage of credentials

> CVE-2024-0056: Microsoft.Data.SqlClient and System.Data.SqlClient SQL Data Provider Security Feature Bypass Vulnerability [@2cve2024]
>
> CWE-420: Unprotected Alternate Channel [@2cwe2024]
>
> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
>
> Severity: HIGH
>
> Priority score: 375

The `Microsoft.EntityFrameworkCore.SqlServer` is a package that serves as a database provider for Entity Framework Core to bs used with a Microsoft SQL Server and Azure SQL (through database migrations) [@2microsoftlearn2024]. Entity Framework (EF) Core is a lightweight, extensible, open-source, and cross-platform of the popular Entity Framework technology, allowing .NET developers to dynamically work with databases using C# objects without the need for manual SQL queries that typically need to be written [@3microsoftlearn2024].

This package, and its affected versions, are vulnerable to unprotected credential storage. In practicality, this means that an attacker can steal authentication credentials required for the database server through man-in-the-middle attacks between the SQL client and the SQL server. It can even occur if a secure connection is established over an encrypted channel such as TLS.

To fix this, the `Microsoft.Data.SqlClient` can be updated or changed to the following versions: `2.1.7`, `3.1.5`, `4.0.5`, and `5.1.3`.

#### 2.2.2.5 `System.Net.Http` – Denial of Service (DoS)

> CVE-2017-0247 [@cve2017]
>
> CWE-245: 7PK - Security Features [@3cwe2023]
>
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

The `System.Net.Http` NuGet package serves as a programming interface for modern HTTP applications, including all the necessary HTTP components required to consume web services over HTTP [@2nuget2024]. It allows HTTP components to be used by both clients and servers for understanding HTTP headers.

Versions of this package that are affected are vulnerable to Denial of Service (DoS) attacks meaning that ASP.NET Core will fail to properly validate web requests. Attackers can use the `TestEncoder.EncodeCore` function in the `System.Text.Encodings.Web` package as part of `System.Net.Http` to trigger a denial of service attack by utilising a failure in code logic that incorrectly calculates the length of 4-byte characters in Unicode.

The vulnerability can be remediated by updating or changing the `System.Net.Http` package to either version `4.1.2` or `4.3.2`.

#### 2.2.2.6 `System.Net.Http` – Improper certificate validation

> CVE-2017-0248 [@2cve2017]
>
> CWE-287: Improper Authentication [@3cwe2024]
>
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

Affected versions of the `System.Net.Http` are vulnerable to improper certificate validation. Attackers can therefore bypass taggings such as "Enhanced Security Usage" when an invalid certificate is presented for a specific use.

The vulnerability can be avoided by updating or changing the `System.Net.Http` package to either version `4.1.2` or `4.3.2`.

#### 2.2.2.7 `System.Net.Http` – Information exposure

> CVE-2018-8292 [@cve2018]
>
> CWE-200: Exposure of Sensitive Information to an Unauthorized Actor [@4cwe2023]
>
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

The `System.Net.Http` package is vulnerable to information exposure, specifically HTTP authentication information from outbound requests that encounter an HTTP redirect. An attacker who manages to exploit this vulnerability can compromise the application further through the leaked information.

This vulnerability can be mitigated by updating or changing the `System.Net.Http` package to the following versions: `2.0.20710`, `4.0.1-beta-23225` (although `beta` packages are not recommended – serves as a temporary fix), `4.1.4`, and `4.3.4`.

#### 2.2.2.8 `System.Text.RegularExpressions` – Regular Expression Denial of Service (ReDoS)

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

`System.Text.RegularExpressions` is an implementation of a regular expressions (RegEx) engine [@3nuget2024], which is an engine that takes a sequence of characters and returns a matching pattern text.

RegEx engines, such as this one, are commonly vulnerable to Regular Expression Denial of Service (ReDoS) attacks due to the improper processing of RegEx strings. It means that an attacker relies on the fact that a large majority of ReGex engine implementations may reach rare but extreme cases that cause them to function very slowly, at an exponential rate [@2owasp2024]. An attacker can therefore use a program or tool to manipulate RegEx engines to enter such states, causing them to hang for a very long time.

The vulnerability for this package is addressed through its subsequent version update of `4.3.1`.

#### 2.2.2.9 `System.Net.Http` – Privilege escalation

> CVE-2017-0249 [@3cve2017]
>
> CWE-269: Improper Privilege Management [@5cwe2023]
>
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

This affected versions of the `System.Net.Http` package leave it vulnerable to privilege escalation due to the improper sanitisation of any web requests. Privilege escalation is a type of attack in which an attacker can gain access elevated resource access that is normally restricted to the average application user.

The versions `4.1.2` and `4.3.2` of the `System.Net.Http` properly resolve this vulnerability.

#### 2.2.2.10 `Microsoft.IdentityModel.JsonWebTokens` and `System.IdentityModel.Tokens` – Resource exhaustion

> Introduced through:
>
> - `Microsoft.EntityFrameworkCore.SqlServer@7.0.12`
> - `Microsoft.Data.SqlClient@5.1.1`
> - `Microsoft.IdentityModel.JsonWebTokens@6.24.0` and `System.IdentityModel.Tokens.Jwt@6.24.0`
>
> Severity: MEDIUM
>
> Priority score: 340

Both packages which have versions that are affected are vulnerable to resource extensions when processing JSON Web Encryption (JWE) tokens that have a high compression ratio. An attacker then therefore utilise this to cause excessive memory location and processing time during the decompression phase leading to an eventual denial-of-service. However, this is only a possible exploit if the attacker has access to the Microsoft Entra ID's (IDP) registered public encrypt key.

The updated versions for both packages, i.e. `5.7.0`, `6.34.0`, and `7.1.2`, patch this vulnerability.

#### 2.2.2.11 System.Net.Http – Authentication bypass

> CVE-2017-0256 [@4cve2017]
>
> CWE-20: Improper Input Validation [@6cwe2023]
>
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

The ASP.NET Core HTTP framework, through the `System.Net.Http` NuGet package, does not properly sanitise the "Web Request Handler" component. This means that attackers are able to spoof and mimic legitimate HTTP web requests and therefore use this to bypass the application's authentication framework.

The vulnerability is addressed in the versions `4.1.2` and `4.3.2` of the `System.Net.Http` library.

## 2.3 Dynamic Application Security Testing (DAST)

<!-- 500 words maximum -->

### 2.3.1 Methodology

<!-- 100 words maximum -->

DAST involves testing and running the application after it has been deployed to a staged environment and identifying potential security vulnerabilities through simulating real-world attacks. Using the OWASP Zed Attack Proxy (ZAP) tool [@owaspzap2024], the DAST scan was performed on the ASP.NET Core C# application. The ZAP tool acts as a type of proxy that intercepts and inspects the application's HTTP requests. This is to detect vulnerabilities, such as flaky authentication frameworks, flawed code injections, data exposure points, and security misconfigurations [@owasptop102021].

### 2.3.2 Vulnerabilities identified

#### 2.3.2.1 Content Security Policy (CSP) header not set

> CWE-693: Protection Mechanism Failure [@7cwe2023]
>
> Risk level: MEDIUM
>
> Number of instances: 2

The Content Security Policy (CSP) header is an added layer of security (defense-in-depth) that aids in the detection and mitigation of specific attacks, those being mainly Cross-Site Scripting (XSS) and data injection attempts [@w3c2024]. The primary goal of this attack could range from data theft to site defacement or even malware distribution. A CSP provides a set of standard HTTP headers that website owners can use to declare approved content sources that browsers should allow to load on that page [@foundeo2023]. This includes content types such as JavaScript, CSS, HTML frames, fonts, images, and embedded objects (video and audio files, Java applets, ActiveX).

A simple solution for this would be to add a CSP header to the application web server, but more specific solutions include:

- Restricting inline scripts, to prevent injection attacks [@owaspcheatsheetseries2024].
- Restricting remote script execution, by preventing the page from executing scripts from other suspicious web servers for injection attacks [@owaspcheatsheetseries2024].
- Restricting any unsafe JavaScript logic that the attacker can exploit [@owaspcheatsheetseries2024].
- Restricting HTML form submissions and instead, use a more secure form framework [@owaspcheatsheetsseries2024].
- Restricting HTML objects to prevent attackers from injecting their own malicious executables, such as deprecated or legacy executables i.e. Java/Flash player [@owaspcheatsheetsseries2024].

#### 2.3.2.2 Proxy disclosure

> CWE-200: Exposure of Sensitive Information to an Unauthorized Actor [@4cwe2023]
>
> Risk level: MEDIUM
>
> Number of instances: 4

Any proxy servers that the application is running were detected and fingerprinted by the ZAP tool. It occurs when `TRACE` and/or `TRACK` methods are enabled on both the proxy and origin web servers [@stackhawk2024]. If these methods are enabled, an attacker can gain access to sensitive information about the software and services running on the server to get information for further attacks, by sending specific requests [@stackhawk2024].

This information helps the attacker to determine the following information:

- A list of targets to construct an attack against the application.
- The presence or absence of any other proxy-based components that may cause attacks against the application can be detected, prevented, or mitigated.
- Any vulnerabilities on the proxy server that also compromise the application.

A straightforward solution to this would be to disable the `OPTIONS` method on the proxy server used by the application, as well as both the `TRACK` and `TRACE` methods [@zaproxy2024]. Furthermore, web and application servers should implement custom error pages to prevent fingerprinting, so server error information is not leaked to the attacker for profiling purposes [@zaproxy2024].

#### 2.3.2.3 Application error disclosure

> CWE-200: Exposure of Sensitive Information to an Unauthorized Actor [@4cwe2023]
>
> Risk level: LOW
>
> Number of instances: 2

Any pages that contain too verbose of an error message may potentially contain information such as file location or server software information that the attacker could use to profile the application resources with [@2zaproxy2024]. However, it is important to note that this vulnerability can also be a false positive – if the error message is found inside a documentation page for example. Such information such as API key credentials, resource location, internal web server configurations, or user data is at risk [@scanrepeat2024].

Like in [Section A, 2.3.2.2 Proxy disclosure](#2322-proxy-disclosure), implementing generic, custom error pages minimises the information available for the attacker on the client side, but verbose error logging on the server side for the developer [@scanrepeat2024]. This allows error references to still be available, without exposing any sensitive information.

#### 2.3.2.4 Permissions Policy Header not set

> CWE-693: Protection Mechanism Failure [@7cwe2023]
>
> Risk level: LOW
>
> Number of instances: 2

The Permissions Policy Header, similar to the Content Security Policy Header from [Section A, 2.3.2.1](#2321-content-security-policy-csp-header-not-set), is another added layer of security that governs the restriction from unauthorised access or usage of browser and client features, such as camera, microphone, full screen, and location [@3zapproxy2024]. The policy header details user privacy by limiting the set of client device features that web resources can use via a standard set of HTTP headers that website owners can use to limit access [@3zapproxy2024].

To fix this, the internet-facing components (web server, application server, load balancer, etc.) should have a Permissions Policy Header configured with secure settings. The `directive` setting governs what client feature is enabled, and the `allowlist` setting either permits or denies access [@mdn2024].

#### 2.3.2.5 Strict-Transport-Security Header not set

> CWE-319: Cleartext Transmission of Sensitive Information [@4cwe2024]
>
> Risk level: LOW
>
> Number of instances: 4

Another security header similar to the previous two ([2.3.2.1 Content Security Policy (CSP) Header](#2321-content-security-policy-csp-header-not-set) and [2.3.2.4 Permissions Policy Header](#2324-permissions-policy-header-not-set)) is a security layer that informs browsers (and complying user agents) that HTTPS site access should be reinforced, and any future HTTP connection attempts are to be converted to HTTPS [@2mdn2024]. However, if this is not set, any visitors to the website can initially communicate with the unencrypted HTTP version before redirection to HTTP occurs. This creates a small window of opportunity for man-in-the-middle attacks whereby users could potentially be redirected to a malicious replica of the website instead of the secure original version [@2mdn2024].

The application's web server and the load balancer will need to be configured to enforce a Strict-Transport-Security Header in any HTTP responses they make.

# 3 Section B: Software automation

## 3.1 Introduction

<!-- 100 words maximum -->

### 3.1.1 Purpose

This section's purpose is to demonstrate the implementation of the CI/CD (Continuous Integration/Continuous Deployment) pipeline for the ASP.NET Core C# web application, Pacific Tours. The automation covers the full software development life cycle, ensuring that builds are consistently tested and deployed – all while aligning with DevSecOps principles [@paloalto2024].

### 3.1.2 Tools and technologies used

#### 3.1.2.1 GitHub Actions

![](Docs/CcseCw2Report/Images/github-actions-logo.png)

#### 3.1.2.2 Docker

![](Docs/CcseCw2Report/Images/docker-logo.png)

#### 3.1.2.3 Google Cloud Platform

![](Docs/CcseCw2Report/Images/google-cloud-platform-logo.png)

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

## 3.4 Vulnerability comparison with [Section A](#2-section-a-software-security-vulnerabilities)

### 3.4.1 Static Application Security Testing (SAST)

No additional vulnerabilities (other than those documented in [Section A, 2.3.2](#232-vulnerabilities-identified)) were detected during the CI/CD pipeline `sast` job.

### 3.4.2 Dynamic Application Security Testing (DAST)

A few additional vulnerabilities, aside from the ones in [Section A, 2.2.2](#222-vulnerabilities-identified), were appended to the OWASP ZAP scan report.

#### 3.4.2.1 Missing anti-clickjacking header

> CWE-1021: Improper Restriction of Rendered UI Layers or Frames [@9cwe2023]
>
> Risk level: MEDIUM
>
> Number of instances: 6

#### 3.4.2.2 Vulnerable JavaScript library

> CWE-829: CWE-829: Inclusion of Functionality from Untrusted Control Sphere [@10cwe2023]
>
> Risk level: MEDIUM
>
> Number of instances: 1

#### 3.4.2.3 Cookie without secure flag

> CWE-614: Sensitive Cookie in HTTPS Session Without 'Secure' Attribute [@11cwe2023]
>
> Risk level: LOW
>
> Number of instances: 2

#### 3.4.2.4 Dangerous JavaScript functions

> CWE-749: Exposed Dangerous Method or Function [@12cwe2023]
>
> Risk level: LOW
>
> Number of instances: 1

#### 3.4.2.5 X-content-type-options header missing

> CWE-693: Protection Mechanism Failure [@7cwe2023]
>
> Risk level: LOW
>
> Number of instances: 11

# 4 Appendices

# 5 References
