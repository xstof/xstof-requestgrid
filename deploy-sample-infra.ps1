param (
    [string]$RG, 
    [string]$Location = "westeurope",
    [string]$ViewerWebAppPrefix
)

# Register EventGrid provider:
Register-AzResourceProvider -ProviderNamespace Microsoft.EventGrid

# Create Resource Group:
New-AzResourceGroup -Name $RG -Location $Location

# Create Event Grid topic:
$TopicName = "xstof-requestgrid-events"
New-AzEventGridTopic -ResourceGroupName $RG -Location $Location -Name $TopicName

# Deploy web app for displaying event grid messages:
New-AzResourceGroupDeployment `
  -ResourceGroupName $RG `
  -TemplateUri "https://raw.githubusercontent.com/Azure-Samples/azure-event-grid-viewer/master/azuredeploy.json" `
  -siteName $ViewerWebAppPrefix `
  -hostingPlanName "$ViewerWebAppPrefix-Plan"

# Subscribe the viewer to the topic:
$Endpoint="https://$ViewerWebAppPrefix.azurewebsites.net/api/updates"
New-AzEventGridSubscription `
  -EventSubscriptionName requestgrid-viewer-subscription `
  -Endpoint $Endpoint `
  -ResourceGroupName $RG `
  -TopicName $TopicName
