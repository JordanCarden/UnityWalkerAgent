# Unity Walker Agent
A Unity ML-Agents project teaching a multi-joint Agent to walk toward a target.

## Prerequisites
Unity 2021.3 (or later) with ML-Agents package installed

Python 3.8+

## Quickstart
Clone the repo
```bash
git clone https://github.com/YourUser/UnityWalkerAgent.git
cd UnityWalkerAgent
```

## Create and activate a Python virtual environment
```bash
python3 -m venv .venv
source .venv/bin/activate
```

## Install Python dependencies
```bash
pip install --upgrade pip
pip install -r requirements.txt
```

## Open in Unity
Launch Unity Hub or the Unity Editor  
Add or open the UnityWalkerAgent folder  
Double click the scene contained Assets/Scenes  

## Train the Agent
```bash
mlagents-learn config.yaml --run-id=run_name --force
```
In Unity, press Play

## Or Load Our Model
First open the project in the Unity Editor and locate the Assets/Models folder in the Project window. 
Find the .onnx file you wish to apply (for example, 16agent.onnx or 4agent.onnx). 
Click on the Body GameObject in the Hierarchy. 
In the Inspector, drag the .onnx asset into the Model field of the Behavior Parameters component. 
Enter Play mode. The agent will immediately begin using the selected pre-trained network.
