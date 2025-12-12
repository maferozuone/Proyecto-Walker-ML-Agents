🦵 Walker ML-Agents – Proyecto de Locomoción Bípeda
Aprendizaje por Refuerzo Profundo en Unity + ML-Agents

Este repositorio contiene un proyecto completo para entrenar y evaluar un agente bípedo (Walker) en Unity utilizando ML-Agents, comparando el rendimiento de los algoritmos PPO y SAC, junto con diferentes configuraciones de recompensas.

El objetivo principal es estudiar cómo influye el diseño de recompensas y el algoritmo seleccionado en la estabilidad, velocidad y naturalidad de la locomoción del agente.

📁 Estructura del Repositorio
📦 Proyecto
 ┣ Assets/               → Modelos, scripts y configuraciones de Unity
 ┣ Scenes/               → Escena principal del Walker
 ┣ Scripts/
 ┃ ┗ WalkerAgent.cs      → Lógica RL del agente
 ┣ config/
 ┃ ┣ WalkerPPO.yaml      → Configuración del algoritmo PPO
 ┃ ┗ WalkerSAC.yaml      → Configuración del algoritmo SAC
 ┣ results/
 ┃ ┣ export_csv.py       → Exporta logs de TensorBoard a CSV
 ┃ ┣ plot_runs.py        → Genera gráficas (avg, max, std, comparaciones)
 ┃ ┗ (carpetas de runs)  → Resultados de entrenamiento
 ┣ Builds/               → Build del entorno Unity
 ┗ README.md

🛠️ Requisitos
Software necesario

Unity 2022.x (compatible con ML-Agents 2.x)

Python 3.9 (recomendado)

ML-Agents Toolkit (mlagents y mlagents-envs)

PyTorch

TensorBoard

(Opcional) Matplotlib, Pandas para análisis

⚙️ Instalación del entorno ML-Agents
python -m venv mlagents-env
mlagents-env\Scripts\activate

pip install mlagents==0.30.0
pip install torch torchvision torchaudio
pip install tensorboard
pip install matplotlib pandas

🎮 Uso del Proyecto en Unity

Abrir Unity Hub

Seleccionar Open Project

Elegir la carpeta principal del repositorio

Abrir la escena:

Assets/Scenes/WalkerScene.unity


Dentro encontrarás:

El Walker

El objetivo (Target)

Sensores

Articulaciones

Comportamientos ML-Agents

🤖 Entrenamiento del Agente
Entrenar con PPO
mlagents-learn config/WalkerPPO.yaml --run-id=WalkerPPO --env="Builds/Walker.exe" --no-graphics

Entrenar con SAC
mlagents-learn config/WalkerSAC.yaml --run-id=WalkerSAC --env="Builds/Walker.exe" --no-graphics


Los resultados aparecerán en:

results/WalkerPPO/
results/WalkerSAC/

📊 Visualización del Entrenamiento (TensorBoard)
tensorboard --logdir results


Abrir en navegador:

http://localhost:6006

📤 Exportar resultados a CSV

Desde la carpeta results/:

python export_csv.py


El script:

lee cada carpeta con logs

extrae las recompensas

genera un CSV por run

📈 Generar gráficas comparativas
python plot_runs.py


Este script genera automáticamente:

Recompensa promedio (moving average)

Recompensa máxima (max per window)

Desviación estándar

Comparación entre runs seleccionadas

Gráficas individuales por cada run

Y guarda las imágenes en:

results/*.png

🎯 Uso del modelo entrenado

Una vez entrenado, Unity generará un archivo .nn.
Puedes cargarlo en:

Behavior Parameters → Model


Así puedes correr el agente sin entrenamiento.

📌 Objetivos del proyecto

Comparar PPO vs SAC en locomoción bípeda

Estudiar cómo el diseño de recompensas afecta el aprendizaje

Obtener movimientos más naturales y estables

Analizar métricas cuantitativas mediante gráficas

Evaluar desempeño bajo distintas condiciones

👨‍💻 Autor

Manuel Fernando Rocha Zuleta, Miguel Enrique Galindo Florez
Universidad Nacional de Colombia – Sede La Paz
Programa de Ingeniería Mecatrónica
