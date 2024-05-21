# 接收两个float 参数
param(
    [float]$x,
    [float]$y
)

#  输出x,y 相对于 (494,878)归一化后的坐标
$res = "$([math]::Round($x/494,4))f, $([math]::Round($y/878,4))f";

Write-Output $res

# copy to clipboard
$res | clip