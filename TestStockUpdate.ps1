# --- Configuration ---
# Replace the PORT and GUID with your actual running API values
$port = "7169" 
$guid = "f6c260cd-ff74-44f5-9072-bd29b6739eaf"

Write-Host "Starting bulk stock update test..." -ForegroundColor Cyan

# Loop from 1000 down to 0 to test various quantities
1000..0 | ForEach-Object {
    $currentQuantity = $_
    $body = "{`"newQuantity`": $currentQuantity}"
    
    try {
        # Sending asynchronous request to the v2 API
        Invoke-RestMethod -Method Post `
          -Uri "https://localhost:$port/api/v2/products/$guid/stock-async" `
          -ContentType "application/json" `
          -Body $body
        
        Write-Host "Sent request #$currentQuantity with quantity $currentQuantity - Status: 202 Accepted" -ForegroundColor Green
    }
    catch {
        # Catching validation errors (e.g., 400 Bad Request for negative numbers)
        Write-Host "Failed request #$currentQuantity - Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}

Write-Host "`nTest complete. Check the API console for background worker logs." -ForegroundColor Cyan