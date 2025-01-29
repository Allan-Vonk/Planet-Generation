# Planet Generator Project Roadmap

## Project Goals
- **Objective:** Develop a framework for a planet generator to be published on the Unity Asset Store.
- **Key Features:**
  - Implement hardware acceleration for planet computation with LODs.
  - Develop a chunk saving system for persistence.
  - Implement the extended marching cubes lookup table to fix the seams.

## Current State
- **Progress:**
  - Basic planet generation using Perlin noise and marching cubes algorithm.
  - LODs managed with an octree structure.
  - Initial GPU-based Perlin noise sampling implemented.
  - Expermented with layered perlin noise.
  - Got a nice formula for subdividing the chunks.

## Pending Features
- **Chunk Saving System:** Implement a system for saving and loading chunks to ensure persistence.
- **GPU Acceleration:** Develop a robust system to handle GPU acceleration for computations.
- **Extended Marching Cubes:** Fully implement the extended marching cubes lookup table to address seam issues.

## Performance Optimization
- **CPU Overload:** Offload calculations to the GPU to reduce CPU load.
- **RAM Usage:** Optimize data per chunk to improve dynamic loading and reduce memory footprint.

## Cross-Platform Considerations
- **Target Platforms:** Focus on PCs with strong GPUs for acceleration, but maintain CPU compatibility for broader accessibility.

## Testing and Debugging
- **Current Strategy:** Utilize a test scene with a player character for navigation and system testing.
- **Enhancements:** Develop more comprehensive testing scenarios to ensure robustness.

## Art and Assets
- **Future Needs:** Plan for terrain textures and environmental assets (e.g., trees) for potential game development.

## Team and Collaboration
- **Current Team:** Solo development effort.
- **Collaboration:** Manage all aspects independently, with potential for future collaboration if needed.

## Timeline and Milestones
- **Completion Goal:** Aim to finish by the end of the school year, with ongoing development as needed.
- **Milestones:**
  - Implement chunk saving system.
  - Complete GPU acceleration integration.
  - Finalize extended marching cubes implementation.

## Risk Management
- **Challenges:**
  - Synchronization between GPU and CPU.
  - Efficient chunk management with GPU.
- **Mitigation Strategies:**
  - Research and apply best practices for GPU-CPU synchronization.
  - Incremental testing and optimization of chunk handling.
