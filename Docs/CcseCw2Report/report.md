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

The second section, [Section B](#3-section-b-software-automation), covers the practical implementation of the CI/CD pipeline using GitHub's CI/CD platform, GitHub actions [@github2024] as a way to automate the build, testing, and deployment process. The pipeline will also include the aforementioned SAST and DAST tools used in [Section A](#2-section-a-software-security-vulnerabilities) in alignment with DevSecOps practices [@paloalto2024]. Stages such as Docker build, image creation, and deployment to a cloud platform are also detailed.

# 2 Section A: Software security vulnerabilities

<!-- 1200 words maximum -->

## 2.1 Introduction

<!-- 100 words maximum -->

### 2.1.1 Purpose

<!-- 50 words maximum -->

### 2.1.2 Tools used

<!-- 50 words maximum -->

## 2.2 Static Application Security Testing (SAST)

<!-- 400 words maximum -->

### 2.2.1 Methodology

<!-- 100 words maximum -->

### 2.2.2 Vulnerabilities identified

<!-- 200 words maximum -->

### 2.2.3 Remediation

<!-- 100 words maximum -->

## 2.3 Dynamic Application Security Testing (DAST)

<!-- 400 words maximum -->

### 2.3.1 Methodology

<!-- 100 words maximum -->

### 2.3.2 Vulnerabilities identified

<!-- 200 words maximum -->

### 2.3.3 Remediation

<!-- 100 words maximum -->

## 2.4 Potential vulnerabilities

<!-- 300 words maximum -->

### 2.4.1 Vulnerability description

<!-- 200 words maximum -->

### 2.4.2 Remediation

<!-- 100 words maximum -->

# 3 Section B: Software automation

<!-- 1250 words maximum -->

## 3.1 Introduction

<!-- 100 words maximum -->

### 3.1.1 Purpose

<!-- 50 words maximum -->

### 3.1.2 Tools and technologies used

<!-- 50 words maximum -->

## 3.2 CI/CD pipeline implementation

<!-- 500 words maximum -->

### 3.2.1 Pipeline overview

<!-- 100 words -->

<!-- Add headings for pipeline stages here -->
<!-- 100 words maximum for each pipeline stage -->

## 3.3 Security testing in CI/CD pipeline

<!-- 500 words maximum -->

### 3.3.1 Methodology

<!-- 150 words maximum -->

### 3.3.2 Results and findings

<!-- 250 words maximum -->

### 3.3.3 Comparison with [Section A](#2-section-a-software-security-vulnerabilities)

<!-- 100 words maximum -->

# 4 Appendices

# 5 References
