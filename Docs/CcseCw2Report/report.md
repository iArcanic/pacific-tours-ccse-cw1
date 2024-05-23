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

# GitHub repository

> [https://github.com/iArcanic/pacific-tours-ccse-cw1](https://github.com/iArcanic/pacific-tours-ccse-cw1)

# 1 Executive summary

This report presents the results from the security vulnerability testing as well as the implementation of a CI/CD pipeline for the ASP.NET C# application developed for the Pacific Tours hotel company.

The report is split into two sections.

The first section, [Section A](#2-section-a-software-security-vulnerabilities), showcases the results of Static Application Security Testing (SAST) using Synk [@snyk2024] and Dynamic Application Security Testing (DAST) using OWASP ZAP [@owaspzap2024]. The identified vulnerabilities are described in this report along with the steps recommended to remediate them. If a lack of vulnerabilities is found, then potential vulnerabilities with their mitigations are explored and mapped to OWASP guidelines [@owasptop102021].

The second section, [Section B](#3-section-b-software-automation), covers the practical implementation of the CI/CD pipeline using GitHub's CI/CD platform, GitHub actions [@github2024] as a way to automate the build, testing, and deployment process. The pipeline will also include the aforementioned SAST and DAST tools used in [Section A](#2-section-a-software-security-vulnerabilities) in alignment with DevSecOps practices [@paloalto2024]. Stages such as Docker build, image creation, and deployment to a cloud platform are also detailed. Furthermore, it also compares and contrasts any additional vulnerabilities

# 2 Section A: Software security vulnerabilities

## 2.1 Introduction

### 2.1.1 Purpose

The purpose of this section is to identify any security vulnerabilities in the codebase of the ASP.NET C# project through both static and dynamic analysis. Through this, secure development practices as recommended by OWASP [@owasp2024] are considered. This can help in mitigating potential security risks in the codebase.

### 2.1.2 Tools used

The SAST scanning was performed using Synk [@snyk2024] and the DAST analysis was carried out by OWASP ZAP [@owaspzap2024]. These industry-grade tools allow for detailed vulnerability auditing whilst adhering to security practices defined by the National Institute of Standards and Technology (NIST) [@nist2024].

## 2.2 Static Application Security Testing (SAST)

### 2.2.1 Methodology

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

The ASP.NET Core HTTP framework, through the `System.Net.Http` NuGet package, does not properly sanitise the "Web Request Handler" component. This means that attackers can spoof and mimic legitimate HTTP web requests and therefore use this to bypass the application's authentication framework.

The vulnerability is addressed in versions `4.1.2` and `4.3.2` of the `System.Net.Http` library.

## 2.3 Dynamic Application Security Testing (DAST)

### 2.3.1 Methodology

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

### 3.1.1 Purpose

This section's purpose is to demonstrate the implementation of the CI/CD (Continuous Integration/Continuous Deployment) pipeline for the ASP.NET Core C# web application, Pacific Tours. The automation covers the full software development life cycle, ensuring that builds are consistently tested and deployed – all while aligning with DevSecOps principles [@paloalto2024].

### 3.1.2 Tools and technologies used

#### 3.1.2.1 GitHub Actions

GitHub Actions is a continuous integration/continuous deployment (CI/CD) pipeline technology that utilises the code in a GitHub repository to automate the processes of building, testing, and deploying [@2github2024]. It uses the idea of "workflows" that trigger the automated processes upon different repository events [@2github2024], such as upon creating a pull request, pushing commits, creating a new branch, and so on.

The reasons as to why GitHub Actions was used is as follows:

- **Seamless integration**: this ASP.NET Core C# web application is hosted on a GitHub repository, so it makes logical sense to use the platform's very own CI/CD automation technology as it is seamlessly integrated with the code infrastructure.
- **Customisability**: it is also highly customisable, being able to run the pipeline on a variety of different architectures through their virtual machines [@2github2024].
- **Cost**: the cost of using GitHub Actions is little to none, having a very generous free tier [@3github2024] that covers most usage requirements.
- **Pre-built actions**: they have a wide variety of already built-in functions via the "GitHub Marketplace" that most typical code bases need [@benvengu2023].
- **Security**: support encryption and masking of sensitive information such as API credentials, keys, and so on through GitHub Actions Secrets.
- **Developer support**: GitHub Actions has a very large and active community, meaning that any errors are likely to be reported and fixed quickly, as well as the ability to share and use pre-build actions [@benvengu2023].

#### 3.1.2.2 Docker

Docker is a platform that allows for the consistent development, shipping, and running of applications [@docker2024]. It helps to separate the applications from the resources they require, allowing developers to only focus on what is essential to the development process. The Docker concepts and methodologies offer a variety of features that are beneficial for the building and deployment of this ASP.NET Core C# web application:

- **Containers**: Docker uses the concept of containers, which allows each service to be individually managed, meaning that resources can be efficiently allocated as needed [@preeth2015].
- **Container isolation**: each Docker container is isolated, helping to maintain the overall security of the system as the application is confined to its execution environment [@bui2015].
- **Architecture variety**: similar to [3.1.2.1 GitHub Actions](#3121-github-actions), Docker containers are capable of running a variety of different architectures such as Linux, Windows, MacOS, and more.
- **Pipeline integrtation**: containers can easily integrate with the CI/CD workflow, allowing code to be fixed of bugs, containerised, and redeployed to the test environment, then pushed to production [@docker2024].
- **Cost**: Docker has a very generous free tier which has near to unlimited usage on any device.

#### 3.1.2.3 Google Cloud Platform (GCP)

Google Cloud Platform (GCP) is a set of various physical assets, those being computers, virtual resources, servers, and hard disk drives all being serves in the form of virtual machines (VMs) rhat are held in various data centers across different regional locations [@google2024]. GCP is therefore essentially a public cloud vendor that offers a suite of computing services that are globally accessible [@knox2023].

The following cloud resources that GCP offers make it suitable for this ASP.NET Core C# web application's components:

- **Google Artifact Registry (GAR)**: a collection of repositories suitable for the storage of Docker container images, preparing them for containers to be deployed to the web [@2google2024].
- **Google Cloud SQL**: a fully managed relational database compatible with database software such as MySQL, PostgreSQL, and SQL server for a deployed web application[@3google2024].
- **Google Cloud Run** a managed compute platform that uses the Docker containers from GAR and runs them to be publically accessible on the internet using Google's scalable infrastructure [@4google2024].

## 3.2 CI/CD pipeline implementation

The CI/CD pipeline for the ASP.NET Core C# web application is implemented using a GitHub Actions workflow consisting of several jobs described in a YAML Ain't Markup Language (YAML) file. The below jobs explain automated pipeline processes such as building, testing, deploying, and so on. Proof from the GitHub Actions and Google Cloud Platform consoles (if necessary) will be provided to demonstrate that each job functions as expected and passes, fulfilling the CI/CD of the application.

The specific security testing pipeline jobs (i.e. `sast` and `dast`) are explained under [Section B: 3.3 Security Testing in CI/CD Pipeline](#33-security-testing-in-cicd-pipeline).

The complete contents of this file can be found at [`pacific-tours-ccse-cw1/.github/workflows/ci-cd.yml`](https://github.com/iArcanic/pacific-tours-ccse-cw1/blob/main/.github/workflows/ci-cd.yml).

### 3.2.1 `test`

The `test` job is the first job in the pipeline that is responsible for checking out the source code, setting up the relevant .NET SDK version, restoring any project dependencies, and then running all available unit tests for their ASP.NET Core C# web application. Since this is the first job, it ensures that the application works as intended without breaking, before moving on to the subsequent pipeline jobs.

```yaml
test:
  runs-on: windows-latest
```

It is important to set the GitHub runner OS to the latest Windows runner, as the native OS for an ASP.NET C# application is designed for the Windows environment.

```yaml
- name: Checkout source code
  uses: actions/checkout@v4
```

The source code from the repository is checked out making it available for the next set of steps.

```yaml
- name: Setup .NET SDK
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: ${{ env.DOTNET_CORE_VERSION }}
```

The next step sets up the .NET SDK necessary for building and testing the application. It uses the relevant .NET version specified by the `DOTNET_CORE_VERSION` environment variable. This must match the same .NET version that the actual ASP.NET Core C# web application is running, which in this case, is the `7.0.x` series.

```yaml
- name: Restore project dependencies
  run: dotnet restore ${{ env.SOLUTION_NAME }}
```

Using the installed .NET SDK commands from the previous step, the `dotnet restore` command can be used to restore the project dependencies, NuGet package references, and other project-specific tools mentioned in the `.sln` or solution file (via the `SOLUTION_NAME` environment variable).

```yaml
- name: Test project
  run: dotnet test --no-restore --verbosity normal
```

Finally, via `dotnet test`, the unit tests are executed. The `--no-restore` argument prevents `dotnet restore` from having to run again since the dependencies have already been restored in the previous step, and the `--verbosity normal` argument ensures that sufficient detail is provided in the log to the developer for the debugging process if this step fails.

![`test` job success](Docs/CcseCw2Report/Images/test-job-success.png)

### 3.2.2 `docker-build-and-push-to-gar`

The `docker-build-and-push-to-gar` is a combination of locally building the Docker image for the ASP.NET Core C# web application first on the GitHub Actions runner machine and then pushing it to the Google Artifact Registry (GAR).

```yaml
runs-on: ubuntu-latest
```

As this job will be using the Docker CLI and those respective commands, they have the best compatibility on Linux machines, so `ubuntu-latest` was a logical choice.

```yaml
needs: [sast, test]
```

The `needs` keyword states that the stated set of jobs will need to be successful for this job to run [@4github2024], in this case, the `sast` (see [3.3 Security Testing in CI/CD Pipeline](#33-security-testing-in-cicd-pipeline)) and `test` (see [3.2.1 `test`](#321-test)). Since the Docker image relies upon the source code to be scanned for vulnerabilities and tested, it makes sure that the application has passed those, to prevent any broken or insecure code from being executed.

```yaml
- name: Checkout source code
  uses: actions/checkout@v4
```

The source code from the repository is checked out making it available for the subsequent set of steps.

```yaml
- name: Google authentication
  uses: google-github-actions/auth@v2
  with:
    credentials_json: ${{ env.GCP_SA_KEY }}
```

Since the built Docker image will be pushed to the GAR, a Google Cloud service, GCP needs to verify the identity and authenticity of the client attempting to connect to it, which in this case, is the GitHub Actions runner machine. To do this, the `GCP_SA_KEY` environment variable is a repository secret containing the credentials (in the format of a JSON file) for the Google Service Account (SA) used to interact with the GCP resources [@6google2024].

```yaml
- name: Setup Google Cloud SDK
  uses: google-github-actions/setup-gcloud@v1
  with:
    version: "latest"
    project_id: ${{ env.GCP_PROJECT_ID }}
```

The next step sets up the Google Cloud SDK on the GitHub Actions runner machine to install the necessary CLI tools for interacting with the required GCP services. The `project_id` parameter is the GCP project ID – a project being the collection for the set of GCP resources that will be used [@5google2024].

```yaml
- name: Build Docker Image
    run: |
      echo "Building Docker image..."
      docker build -t \
        ${{ env.GCP_REGION }}-docker.pkg.dev/${{ env.GCP_PROJECT_ID }}/${{ env.GCP_GAR_REPO }}/${{ env.CONTAINER_IMAGE_NAME }}:${{ github.sha }} .
```

This step concerns itself with building the actual image itself using the build instructions from the `Dockerfile` provided in the repository's root (see [`pacific-tours-ccse-cw1/Dockerfile`](https://github.com/iArcanic/pacific-tours-ccse-cw1/blob/main/Dockerfile)). In the `run` block, the `echo "Building Docker image..."` provides an informative message to the developer in the GitHub Actions console. Following on, the `docker build` command builds an image of the application and tags it through the argument `-t` with a value of `${{ env.GCP_REGION }}-docker.pkg.dev/${{ env.GCP_PROJECT_ID }}/${{ env.GCP_GAR_REPO }}/${{ env.CONTAINER_IMAGE_NAME }}:${{ github.sha }}`. The tag includes the GAR URL (`GCP_REGION`, `GCP_PROJECT_ID`, and `GCP_GAR_REPO`), the container image name (`CONTAINER_IMAGE_NAME`), and the current GitHub commit SHA value (`github.sha`).

```yaml
- name: Configure Docker client
  run: |
    gcloud auth configure-docker --quiet
    gcloud auth configure-docker ${{ env.GCP_REGION }}-docker.pkg.dev --quiet
```

Before pushing the Docker image to GAR, this step configures the Docker CLI client to be authenticated with the GAR so that it can interact with it securely. The `--quiet` flag ensures that any user input that is required by these commands is defaulted to the standard values.

```yaml
- name: Push Docker image to Google Artifact Registry
  run: |
    echo "Pushing Docker image to Google Artifact Registry..."
    docker push \
        ${{ env.GCP_REGION }}-docker.pkg.dev/${{ env.GCP_PROJECT_ID }}/${{ env.GCP_GAR_REPO }}/${{ env.CONTAINER_IMAGE_NAME }}:${{ github.sha }}
```

Finally, the Docker image is then pushed to the GAR via the `docker push` command, using the same image tag from the previous Docker build step.

![`docker-build-and-push-to-gar` job success](Docs/CcseCw2Report/Images/docker-build-and-push-to-gar-job-success.png)

![Docker image in GAR repository](Docs/CcseCw2Report/Images/gar-docker-image.png)

### 3.2.3 `deploy`

The `deploy` pipeline job is responsible for deploying the Docker image from the GAR (pushed from the previous step, [3.2.2 `docker-build-and-push-to-gar`](#322-docker-build-and-push-to-gar)) to Google Cloud Run to be publically available via a URL.

```yaml
runs-on: ubuntu-latest
```

This job will run on GitHub Action's latest Ubuntu runner since the commands for this job are generic and do not require any specific architecture.

```yaml
needs: docker-build-and-push-to-gar
```

This job depends on the successful completion of the `docker-build-and-push-to-gar` before it can run, meaning in practicality, it ensures that the Docker image is first built and pushed to the GAR before attempting to deploy it. It also means that the correct and most recent up-to-date version of the Docker image is referenced during deployment.

```yaml
- name: Checkout source code
  uses: actions/checkout@v4
```

As always, the source code from the repository is checked out making it available for the next set of steps.

```yaml
- name: Google authentication
  uses: google-github-actions/auth@v2
  with:
    credentials_json: ${{ env.GCP_SA_KEY }}
```

To use the Google Cloud Platform services, this step authenticates the GitHub Actions runner machine via the `GCP_SA_KEY` JSON credentials file through a Service Account like before.

```yaml
- name: Setup Google Cloud SDK
  uses: google-github-actions/setup-gcloud@v1
  with:
    version: "latest"
    project_id: ${{ env.GCP_PROJECT_ID }}
```

Since the GitHub Actions runner machine needs access to `gcloud` commands, which is the GCP CLI tool, this step sets up the SDK along with the `project_id` that references the according project where all the GCP resources point towards.

```yaml
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

Finally, to deploy the Docker image as a container to Google Cloud run, the `gcloud run deploy` command achieves this. This command includes a lot of flags and features:

- **`gcloud run deploy ${{ env.GCP_CLOUD_RUN_SERVICE }}`**: the name of the Google Cloud Run service to be deployed.
- **`--image`**: a reference to the Docker image being deployed, pulled from the GAR using the image tag from the previous step.
- **`--region`**: the GCP region where the Cloud Run service is deployed.
- **`--allow-unauthenticated`**: anyone on the public internet, i.e. those who are not authenticated by whitelisting their IPs, can access the Cloud Run service (only set for testing purposes, this will change when pushed to production).
- **`--memory`**, **`--cpu`**, **`--min-instances`**, **`--max-instances`**, **`concurrency`**: all of these flags relate to the resources to be allocated (memory, CPU, minimum instances, and maximum instances) to the Cloud Run services (values set to a baseline to be in line with free tier usage).
- **`--add-cloudsql-instances`**: references an existing Cloud SQL instance (i.e. relational SQL database) to be linked to the Google Cloud Run service.
- **`--update-env-vars`**: updates the environment variables inside the Google Cloud Run service, which the Docker container also has access to, which in this case, setting the `ASPNETCORE_ENVIRONMENT` to `Development` (for testing purposes, but this will be changed to `Production` when pushed to the production environment).
- **`--platform`**: states that the service deployment is to be managed by the Google Cloud Run platform.

![`deploy` job success](Docs/CcseCw2Report/Images/deploy-job-success.png)

![Deployed website](Docs/CcseCw2Report/Images/deployed-website.png)

### 3.2.4 `database-migration`

The `database-migration` job concerns running Entity Framework Core (EF Core) database migrations against a Google Cloud SQL instance. This job ensures that upon each push, migrations are applied and that the database schema is updated with any changes made to models or the database context class defined in the ASP.NET Core C# web application.

```yaml
runs-on: ubuntu-latest
```

This job will run on GitHub Action's latest Ubuntu runner. Commands in this job do not require a specific architecture.

```yaml
needs: deploy
```

This job depends on the website being successfully deployed before the database migrations are run. This means that there is no point in continuing the pipeline or running the database migrations in parallel, and makes logical sense to update the schema only if the website is up and running.

```yaml
- name: Checkout source code
  uses: actions/checkout@v4
```

As always, the source code from the repository is checked out making it available for subsequent steps.

```yaml
- name: Setup .NET SDK
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: ${{ env.DOTNET_CORE_VERSION }}
```

The next step sets up the .NET SDK necessary for building and testing the application. It uses the .NET version specified by the `DOTNET_CORE_VERSION` environment variable, matching the same .NET version of the ASP.NET Core C# web application running, i.e. the `7.0.x` series.

```yaml
- name: Install Dotnet Entity Framework CLI tool
  run: |
    dotnet tool install --global dotnet-ef
    dotnet tool restore
```

A specific `dotnet` tool, `dotnet-ef` is required to perform the EF Core database migrations.

```yaml
- name: Download Cloud SQL Auth Proxy
  run: |
    curl -o \
        cloud-sql-proxy https://storage.googleapis.com/cloud-sql-connectors/cloud-sql-proxy/v2.11.0/cloud-sql-proxy.linux.amd64
```

This step downloads the Google Cloud SQL Auth Proxy, which is required to authorise the GitHub Actions runner machine for interaction with the Google Cloud SQL instance, in a secure manner. It uses the `curl` command to get the binary executable from the URL of this package and name it `cloud-sql-proxy`.

```yaml
- name: Make the Cloud SQL Auth Proxy executable
  run: chmod +x cloud-sql-proxy
```

The Google Cloud SQL Auth Proxy package needs to be given the executable permission via the `chmod +x` command, for it to be run.

```yaml
- name: Google authentication
  uses: google-github-actions/auth@v2
  with:
    credentials_json: ${{ env.GCP_SA_KEY }}
```

Like in the previous steps, the GitHub Actions runner machine has to be authenticated to interact with the GCP services, and this is through the Service Account's credentials JSON file referenced via the `GCP_SA_KEY` environment variable.

```yaml
- name: Setup Google Cloud SDK
  uses: google-github-actions/setup-gcloud@v1
  with:
    version: "latest"
    project_id: ${{ env.GCP_PROJECT_ID }}
```

All the relevant Google Cloud CLI commands need to be installed, i.e. the `gcloud` commands, as they will be required in the next set of steps. A reference to the GCP project is given via the `project_id` environment variable which is a collection of resources associated with this project.

```yaml
- name: Start Cloud SQL Auth Proxy
  run: |
    ./cloud-sql-proxy ${{ env.GCP_CLOUD_SQL_INSTANCE }} &
```

Using the previously installed Cloud SQL Auth Proxy, it can now be run to forward and authenticate all outgoing connections from the GitHub Actions runner machine to the Google Cloud SQL instance. The `&` appended at the end of the command is a Linux syntax for pushing the process to the background in another thread, so other commands can run in the foreground [@stackoverflow2012].

```yaml
- name: Run database migrations
  run: |
    dotnet ef database update
  env:
    ASPNETCORE_ENVIRONMENT: Development
    ConnectionStrings__DefaultConnection: ${{ env.CONNECTION_STRING_GITHUB_ACTIONS }}
```

This step runs the Entity Framework Core database migrations via `dotnet ef database update`. Here, the `ASPNETCORE_ENVIRONMENT` is set to `Development` for testing purposes (this will be changed to `Production` when pushed to a production environment), and the `ConnectionStrings__DefaultConnection` to `CONNECTION_STRING_GITHUB_ACTIONS` – a string that contains the Cloud SQL connection instance with user credentials, database, and the hostname.

```yaml
- name: Stop Cloud SQL Proxy
  run: killall cloud-sql-proxy
```

After the database migrations have successfully run, there are no longer going to be any more outgoing connections to the Cloud SQL instance, so there is also no need to keep the proxy running. The `killall` command stops all services with the name of `cloud-sql-proxy`.

![`database-migration` job success](Docs/CcseCw2Report/Images/database-migration-job-success.png)

![Cloud SQL instance database](Docs/CcseCw2Report/Images/cloud-sql-db.png)

## 3.3 Security testing in CI/CD pipeline

### 3.3.1 `sast`

The `sast` job performs Security Application Security Testing (SAST) on the ASP.NET Core C# web application using the Snyk tool. This is to help identify potential security vulnerabilities in the source code before it is built and deployed.

```yaml
runs-on: ubuntu-latest
```

This job runs on the latest Ubuntu architecture on the GitHub Actions runner machine. More specifically, a Linux-based system is required to run the Snyk CLI commands.

```yaml
needs: test
```

The `needs` keyword here ensures that the application code has passed the required unit tests before performing the security analysis on the source code. There would be no reason to perform the SAST scan if the application code is in a broken state.

```yaml
permissions: write-all
```

This job is given write permissions to upload the SAST scan security results to GitHub Code Scanning.

```yaml
- name: Checkout source code
  uses: actions/checkout@v4
```

The first step checks out the source code from the repository so that subsequent steps have access to it.

```yaml
- name: Setup .NET SDK
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: ${{ env.DOTNET_CORE_VERSION }}
```

The GitHub Actions runner machine needs to have access to the `dotnet` set of CLI commands, so this step sets up the .NET SDK with the specified `dotnet-version`, matching the same version as the ASP.NET Core C# web application.

```yaml
- name: Restore project dependencies
  run: dotnet restore ${{ env.SOLUTION_NAME }}
```

The project's dependencies, references to specific tools, and any other requirements need to be restored (installed) via the `dotnet restore` command, by passing the solution name i.e. the `.sln` file.

```yaml
- name: Run Snyk to check for vulnerabilities
  uses: snyk/actions/dotnet@master
  continue-on-error: true
  with:
    args: --sarif-file-output=snyk.sarif
```

Using Snyk's specific .NET action (`snyk/actions/dotnet@master`), this step performs the SAST vulnerability detection on the .NET codebase. The `continue-on-error` flag is set to `true` to ensure that the job succeeds and continues even if Snyk encounters errors during the scan process. At the end of the scan, it outputs all vulnerabilities within a SARIF (Static Analysis Results Interchange Format) file, `synk.sarif`, to be suitable for GitHub Code Scanning.

```yaml
- name: Upload result to GitHub code scanning
  uses: github/codeql-action/upload-sarif@v3
  with:
    sarif_file: snyk.sarif
    category: snyk-sast-analysis
```

The SARIF file from the previous step generated by Snyk uploads this to the GitHub code scanning feature using the `github/codeql-action/upload-sarif@v3` action. The `sarif_file` input is where the path to the Synk SARIF file is passed, and the `category` input, `synk-sast-analysis`, provides a label on the UI for developers to identify the security results.

### 3.3.2 `dast`

The `dast` job performs Dynamic Application Security Testing (DAST) on the final deployed ASP.NET Core C# web application via the OWASP ZAP tool. This is to identify additional vulnerabilities by simulating real-world attacks on the final form of the application.

```yaml
runs-on: ubuntu-latest
```

The job runs on the latest Ubuntu architecture on the GitHub Actions runner machine. Typically `ubuntu-latest` is the standard choice when system specific commands aren't required.

```yaml
needs: deploy
```

The `dast` job needs the ASP.NET C# application to be deployed successfully first via the `deploy` job. This ensures that the application has been deployed to Google Cloud Run before performing the DAST scan.

```yaml
- name: Checkout source code
  uses: actions/checkout@v4
```

As always, the code is first checked out to allow other steps to have access to the source code.

```yaml
- name: Google authentication
  uses: google-github-actions/auth@v2
  with:
    credentials_json: ${{ env.GCP_SA_KEY }}
```

Using the Service Account (SA) linked to the GCP project for this application, the GitHub Actions runner machine has to be authenticated to access the necessary resources. The `GCP_SA_KEY` is a JSON file that is stored as a repository secret, containing credentials.

```yaml
 - name: Setup Google Cloud SDK
  uses: google-github-actions/setup-gcloud@v1
  with:
    version: "latest"
    project_id: ${{ env.GCP_PROJECT_ID }}
```

Another step within this job needs access to the Google Cloud CLI i.e. the `gcloud` commands. This step therefore sets up the Google Cloud SDK based on the resources enabled on the project (referenced via `project_id`).

```yaml
- name: Get Cloud Run Service URL
  id: get-url
  run: |
    URL=$(gcloud run services describe ${{ env.GCP_CLOUD_RUN_SERVICE }} --region=${{ env.GCP_REGION }} --format 'value(status.url)')
    echo "::set-output name=url::$URL"
```

Using the installed `gcloud` commands from the previous step, the URL of the deployed website running through Google Cloud Run is retrieved and set as the job's output via `echo "::set-output name=url::$URL"`. This now means that the `URL` variable is accessible by other steps.

```yaml
- name: Run OWASP ZAP scan
  uses: zaproxy/action-full-scan@v0.10.0
  with:
    target: ${{ steps.get-url.outputs.url }}
```

This step runs the OWASP ZAP tool to perform the DAST scan on the deployed website. The `zaproxy/action-full-scan@v.0.10.0` action requires a target URL (through the `target` input) to run the simulated attacks against. The URL is retrieved from the output of the previous step.

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

This vulnerability concerns either the lack of a Content Security Policy (CSP) header (see [Section A, 2.3.2.1 Content Security Policy (CSP) header not set](#2321-content-security-policy-csp-header-not-set)) with a "frame-ancestors" directive or any X-Frame-Options, to protect against "Clickjacking" attacks. A Clickjacking attack is one which fools users into thinking that they are clicking on one website element, when in reality, they are actually clicking on another [@synopsys2024]. Users think that they are interacting with the web page's genuine UI, but there is another underlying UI in control, i.e. the UI has been redressed [@synopsys2024].

The vulnerability can be addressed in the following ways:

- By enforcing the usage of modern web browsers to be used by the user. This is because they support the CSP and X-Frame-Options HTTP headers, and should be set to be returned by all web pages of the application [@4zapproxy2024].
- Introduce the `frame-ancestors` directive to the CSP header [@2owaspcheatsheetseries2024].
- Preventing the browser from loading external frames by using the `SAMEORIGIN` value or `DENY` if the application pages do not use framing. These are both part of the `FRAMESET` settings [@4zapproxy2024].

#### 3.4.2.2 Vulnerable JavaScript library

> CWE-829: CWE-829: Inclusion of Functionality from Untrusted Control Sphere [@10cwe2023]
>
> Risk level: MEDIUM
>
> Number of instances: 1

An identified JavaScript library (`jquery-validation`) used by one of the client-side pages is vulnerable. This is a package that is responsible for the form validation logic on the client side before the data is submitted to the server, hence reducing server load times [@javatpoint2021].

A simple solution to this vulnerability would be to upgrade to the latest version of `jquery-validation` [@5zapproxy2024].

#### 3.4.2.3 Cookie without secure flag

> CWE-614: Sensitive Cookie in HTTPS Session Without 'Secure' Attribute [@11cwe2023]
>
> Risk level: LOW
>
> Number of instances: 2

The vulnerability details a cookie being set without a secure flag, meaning that it can be accessed via unencrypted connections [@6zapproxy2024]. It can also potentially mean information exposure as an attacker can eavesdrop on the network traffic leading to session tokens, sensitive data, and user credentials to be leaked [@2stackhawk2024]. It can also be a means for man-in-the-middle attacks, by intercepting the communication between the user and the server since the attacker could modify the contents of the cookie or inject malicious custom code for the user's session [@2stackhawk2024]. Furthermore, impersonation attacks can occur if the user's session is hijacked, allowing the attacker to perform actions as the user [@2stackhawk2024].

The vulnerability can be addressed in the following ways:

- Sending cookies that contain sensitive information through an encrypted channel [@6zapproxy2024].
- Set the `secure` flag for cookies [@6zapproxy2024].
- Implement HTTPS connections [@2stackhawk2024].
- Review or update cookie handling policies [@2stackhawk2024].

# 4 References
