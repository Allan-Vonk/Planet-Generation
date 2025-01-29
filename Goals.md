Project Goals:
What are the primary objectives of your project?
Are there specific features or functionalities you aim to implement?

I want to create a framework for a planet generator. wich i can publish on the unity asset store.
To get there i need a solid system that can work with hardware acceleration to compute the planet with lods. To make it actually usable i also need a chunk saving system for persistence.

2. Current State:
What is the current status of your project?
Are there any existing components or systems that need improvement or refactoring?

I can currently generate a planet with a simple perlin noise using a marching cubes algorithm in combination with a octree to create lods, i also have some basic perlin sampling working on the gpu.

3. Pending Features:
What features are still pending or in development?
Are there any new features you plan to add?

I need to implement a chunk saving system for persistence. I also need to implement a system that can handle the gpu acceleration. I also would like fully implement the extended marching cubes lookup table to fix the seams.

4. Performance Optimization:
Are there any known performance bottlenecks?
Do you have plans for optimizing specific systems or components?

The cpu is getting overloaded with the amount of data that needs to be processed. A lot of the calculations can be offloaded to the gpu. 
Theres also the problem with the ram, The dynamic loading is barely enough. i need a way to minimize the data per chunk. Let alone the fact that i still need to make the percistance system work.

Cross-Platform Considerations:
Is your project targeting multiple platforms?
Are there any platform-specific challenges you anticipate?

Mainly focussing on pc's that have good gpus for the gpu acceleration, but it currently runs on the cpu, so anything with a good enough cpu can run it.

6. Testing and Debugging:
What is your current testing strategy?
Are there any areas that require more robust testing or debugging?

I have a simple test scene that i can use to test the system. I have a simple player character that i can use to navigate the planet.

Art and Assets:
Are there any art or asset requirements that need to be addressed?
Do you have a pipeline for asset integration?

In the future if i want to make a game from the framework i do need assets, mostly textures for the terrain, and possible some trees and other objects.

Team and Collaboration:
Are there other team members involved in the project?
How do you plan to manage collaboration and communication?

I am working alone on this one

Timeline and Milestones:
What is your timeline for project completion?
Are there specific milestones you want to achieve along the way?

I would like to get this done by the end of this school year, but this project is just a gift that keeps on giving. so i will be working on it for a while.


10. Risk Management:
Are there any potential risks or challenges you foresee?
How do you plan to mitigate these risks?

I am worried about the synchorization of the gpu and cpu. Getting the chunks to work nicely with the gpu is going to be a challenge.
