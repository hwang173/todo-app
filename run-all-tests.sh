#!/bin/bash

# Run all tests for the todo-app (backend and frontend)
# Usage: ./run-all-tests.sh [backend|frontend|all] [--coverage]

BACKEND_PATH="./backend"
FRONTEND_PATH="./frontend/todo-ui"
TARGET="${1:-all}"
COVERAGE="${2}"

echo "========================================"
echo "Todo App Test Suite"
echo "========================================"

# Run Backend Tests
if [ "$TARGET" == "backend" ] || [ "$TARGET" == "all" ]; then
    echo ""
    echo "[1/2] Running Backend Tests (.NET)..."
    echo "----------------------------------------"
    
    if [ -f "$BACKEND_PATH/TodoApi.Tests/TodoApi.Tests.csproj" ]; then
        dotnet test "$BACKEND_PATH/TodoApi.Tests/TodoApi.Tests.csproj" --verbosity:normal
        if [ $? -ne 0 ]; then
            echo "Backend tests FAILED!"
            exit 1
        fi
        echo "Backend tests PASSED!"
    else
        echo "Test project not found"
        exit 1
    fi
fi

# Run Frontend Tests
if [ "$TARGET" == "frontend" ] || [ "$TARGET" == "all" ]; then
    echo ""
    echo "[2/2] Running Frontend Tests (Angular)..."
    echo "----------------------------------------"
    
    if [ -d "$FRONTEND_PATH" ]; then
        cd "$FRONTEND_PATH"
        
        # Check if node_modules exists, if not run npm install
        if [ ! -d "node_modules" ]; then
            echo "Installing dependencies..."
            npm install
        fi
        
        if [ "$COVERAGE" == "--coverage" ]; then
            npm test -- --coverage
        else
            npm test
        fi
        
        TEST_RESULT=$?
        cd - > /dev/null
        
        if [ $TEST_RESULT -ne 0 ]; then
            echo "Frontend tests FAILED!"
            exit 1
        fi
        echo "Frontend tests PASSED!"
    else
        echo "Frontend directory not found"
        exit 1
    fi
fi

echo ""
echo "========================================"
echo "All Tests Completed Successfully!"
echo "========================================"
