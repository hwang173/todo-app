#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Run all tests for the todo-app (backend and frontend)
.DESCRIPTION
    This script runs both the backend (.NET) tests and frontend (Angular) tests
.EXAMPLE
    .\run-all-tests.ps1
.EXAMPLE
    .\run-all-tests.ps1 -Backend
.EXAMPLE
    .\run-all-tests.ps1 -Frontend
#>

param(
    [switch]$Backend,
    [switch]$Frontend,
    [switch]$Coverage
)

$BackendPath = ".\backend"
$FrontendPath = ".\frontend\todo-ui"

# If no specific target is specified, run all tests
if (-not $Backend -and -not $Frontend) {
    $Backend = $true
    $Frontend = $true
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Todo App Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Run Backend Tests
if ($Backend) {
    Write-Host "`n[1/2] Running Backend Tests (.NET)..." -ForegroundColor Yellow
    Write-Host "----------------------------------------" -ForegroundColor Yellow
    
    $testProject = "$BackendPath\TodoApi.Tests\TodoApi.Tests.csproj"
    
    if (Test-Path $testProject) {
        dotnet test $testProject --verbosity:normal
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Backend tests FAILED!" -ForegroundColor Red
            exit 1
        }
        Write-Host "Backend tests PASSED!" -ForegroundColor Green
    } else {
        Write-Host "Test project not found at $testProject" -ForegroundColor Red
        exit 1
    }
}

# Run Frontend Tests
if ($Frontend) {
    Write-Host "`n[2/2] Running Frontend Tests (Angular)..." -ForegroundColor Yellow
    Write-Host "----------------------------------------" -ForegroundColor Yellow
    
    if (Test-Path $FrontendPath) {
        Push-Location $FrontendPath
        
        # Check if node_modules exists, if not run npm install
        if (-not (Test-Path "node_modules")) {
            Write-Host "Installing dependencies..." -ForegroundColor Cyan
            npm install
        }
        
        if ($Coverage) {
            npm test -- --coverage
        } else {
            npm test
        }
        
        $testResult = $LASTEXITCODE
        Pop-Location
        
        if ($testResult -ne 0) {
            Write-Host "Frontend tests FAILED!" -ForegroundColor Red
            exit 1
        }
        Write-Host "Frontend tests PASSED!" -ForegroundColor Green
    } else {
        Write-Host "Frontend directory not found at $FrontendPath" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "All Tests Completed Successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
