try {

    #Your code here...
    Write-Host "*** WOLFPACK***" -ForegroundColor yellow
    
    # binaries folder files
    $wpFolder = Join-Path $packageFolder "lib\net45"
    $wpFiles = Get-ChildItem $wpFolder

	Write-Host "Relocating package contents to: " $packageFolder

    foreach ($wpFile in $wpFiles) {
        Move-Item $wpFile.FullName $packageFolder
    }
    Write-Host "Binaries moved..."

    # content folder files	
    $wpFolder = Join-Path $packageFolder "content"
	rename-item $wpFolder "_content"
	$wpFolder = Join-Path $packageFolder "_content"
    $wpFiles = Get-ChildItem $wpFolder

    foreach ($wpFile in $wpFiles) {
        Move-Item $wpFile.FullName $packageFolder
    }
    Write-Host "Content moved..."
    
    Write-Host "Cleaning up..."
    Remove-Item (Join-Path $packageFolder "*.nupkg")
    Remove-Item (Join-Path $packageFolder "lib") -recurse -force
    Remove-Item (Join-Path $packageFolder "_content") -recurse -force

    Write-ChocolateySuccess 'Wolfpack'
} catch {
  Write-ChocolateyFailure 'Wolfpack' $($_.Exception.Message)
  throw 
}