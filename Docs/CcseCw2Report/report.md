---
title: "Pacific Tours CCSE-CW2 Report"
author: "2242090"
bibliography: Docs/CcseCw2Report/references.bib
toc: true
toc-title: Table of Contents
toc-depth: 4
csl: Docs/Shared/harvard-imperial-college-london.csl
---

---

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

#### 2.2.2.2 Anti-forgery token validation disabled

<!-- 50 words maximum -->

#### 2.2.2.3 Azure.Identity – Remote Code Execution (RCE)

<!-- 50 words maximum -->

#### 2.2.2.4 Microsoft.Data.SqlClient – Unprotected storage of credentials

<!-- 50 words maximum -->

#### 2.2.2.5 System.Net.Http – Denail of Service (DoS)

<!-- 50 words maximum -->

#### 2.2.2.6 System.Net.Http – Improper certificate validation

<!-- 50 words maximum -->

#### 2.2.2.7 System.Net.Http – Information exposure

<!-- 50 words maximum -->

#### 2.2.2.8 System.Text.RegularExpressions – Regular Expression Denial of Service (ReDoS)

<!-- 50 words maximum -->

#### 2.2.2.9 System.Net.Http – Privilege escalation

<!-- 50 words maximum -->

#### 2.2.2.10 Microsoft.IdentityModel.JsonWebTokens – Resource exhaustion

<!-- 50 words maximum -->

#### 2.2.2.11 System.IdentityModel.Tokens.Jwt – Resource exhaustion

<!-- 50 words maximum -->

#### 2.2.2.12 Azure.Identity – Information exposure through an error message

<!-- 50 words maximum -->

#### 2.2.2.13 System.Net.Http – Authentication bypass

<!-- 50 words maximum -->

### 2.2.3 Remediation

<!-- 250 words maximum -->

#### 2.2.3.1 NuGet.Packaging – Improper access control

<!-- 20 words maximum -->

#### 2.2.3.2 Anti-forgery token validation disabled

<!-- 20 words maximum -->

#### 2.2.3.3 Azure.Identity – Remote Code Execution (RCE)

<!-- 20 words maximum -->

#### 2.2.3.4 Microsoft.Data.SqlClient – Unprotected storage of credentials

<!-- 20 words maximum -->

#### 2.2.3.5 System.Net.Http – Denail of Service (DoS)

<!-- 20 words maximum -->

#### 2.2.3.6 System.Net.Http – Improper certificate validation

<!-- 20 words maximum -->

#### 2.2.3.7 System.Net.Http – Information exposure

<!-- 20 words maximum -->

#### 2.2.3.8 System.Text.RegularExpressions – Regular Expression Denial of Service (ReDoS)

<!-- 20 words maximum -->

#### 2.2.3.9 System.Net.Http – Privilege escalation

<!-- 20 words maximum -->

#### 2.2.3.10 Microsoft.IdentityModel.JsonWebTokens – Resource exhaustion

<!-- 20 words maximum -->

#### 2.2.3.11 System.IdentityModel.Tokens.Jwt – Resource exhaustion

<!-- 20 words maximum -->

#### 2.2.3.12 Azure.Identity – Information exposure through an error message

<!-- 20 words maximum -->

#### 2.2.3.13 System.Net.Http – Authentication bypass

<!-- 20 words maximum -->

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

### 2.3.3 Remediation

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

### 3.2.2 `docker-build-and-push-to-gar`

### 3.2.3 `deploy`

### 3.2.4 `database-migration`

## 3.3 Security testing in CI/CD pipeline

### 3.3.1 `sast`

### 3.3.2 `dast`

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

#### 3.4.2.3 Remediation

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
