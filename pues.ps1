Set-Location D:\Source\Iro

Get-ChildItem -Recurse -File |
    Where-Object {
        $_.Extension -in '.cs', '.csproj', '.sln', '.md' -and
        $_.FullName -notmatch '\\bin\\|\\obj\\|\\.git\\|\\TestResults\\|\\node_modules\\'
    } |
    ForEach-Object {
        $_.FullName.Replace((Get-Location).Path + '\', '')
    } |
    Sort-Object |
    Set-Content .\projektuebersicht.txt