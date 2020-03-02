# Roll-A-Ball AI

**Introduction**

First of all, a brief explanation of what we're going to delve ourselves into: a description of all the work that has been done for this project, from the inception of a gaming environment that would allow to compare two different AI techniques in a meaningful way, followed by the implementation process of the chosen AI techniques and the data that has been gathered from this experience.

**Methodology**

The workflow has been structured as follows in order to gather further insight.

- **Choosing a proper test environment**

The main problem with this part has been the brainstorming process, as there are a lot of possible applications of the techniques that have been taught. Facing a huge roster of different approaches to creating intelligent agents, taking the first steps hasn't been a trivial activity. Although there was a reasonable understanding of every technique, at the beginning of the task what was lacking was the technical expertise that would've allowed to understand how much time would've took to implement each and every technique successfully, within the deadlines. Everything has potential, and every technique can be used to make a good project.

In order to brainstorm ideas, the decision that has been taken was to select one between a few "simple" AI techniques that were deemed easy to implement, upon which could be based the first version of the game, and against which would later be compared a more complex AI.

The possible candidates for the first AI choice were Finite State Machines and more specifically Rule Based Systems, as they appeared really easy to implement. As for the second AI, the choice was Reinforcement Learning.

At the end of the designing process, the result was the following gaming environment.

**Roll-A-Ball**

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image002.jpg)

The rules are pretty simple. The game is played by two players, represented by two spheres. It is set on a board surrounded by walls, upon which we find a collection of objects. These objects are positioned randomly, so that every match is different from another, and represent two different kinds of items the player can interact and collide with.

The first item is a green cube, named Collectible. Interacting with this item increases the player's score by 2 points. The second item, a small purple cube, is called Danger. On collision with the player it will decrease the player's score by 1 point.

The game features a peculiar mechanic: the player with the lowest score can steal one point from the other player, by colliding against it. The collision will push the player with the higher score in the opposite direction, which, if aimed properly, may also send it flying against a Danger item, thus decreasing the score twice, or send it against a Collectible, thus having the opposite effect. The game ends as soon as all the Collectible items have been picked up: as soon as the finishing state is reached, the game declares the victory of the player that has scored more points. Some interesting variants of this game are one with a time limit, where the player's victory would depend on how many points it has before the timer reaches the limit, or one where there are no walls: an eventual victory condition in this case would be to make the enemy fall off the platform, and vice versa for the defeat condition. To avoid overcomplicating the problem at hand, the simplest variant is the one that has been opted for this report.

- **The First AI: Rule Based System**

o **RBS: Design**

In this version of the game one of the players is controlled by an Artificial Intelligence, structured roughly as described in the following picture.    

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image004.png)

The AI is fundamentally relying on a Rule Based System, so its behaviour is deterministic. It has a starting state named "Idle", from which the AI processes all the information in the database, before taking action.

The first information that is checked is if there are Collectible items available on the board. If the condition is false, the AI will check if the Victory conditions have been reached: if the AI has scored more points than the opponent, it will result in a Player Defeat, or a Player Victory otherwise.

If there are still Collectibles available, the AI will check which object is the closest source of reward. If the player's score is higher than the AI's, it will start chasing the player if it's closer than any Collectible item. If the player score is not higher or it is not the closest object, it will be ignored, and the AI will start to go towards the closest Collectible item. The reason why the RBS has been coded this way is to simulate a human player's behaviour: a human player should ideally be able to see every object on the board, but concentrate only on one single task at a time (in this case, the task would be reaching all the rewards on the board).

To be fair though the AI isn't really following the most optimal way of ensuring its victory, and there are indeed certain game mechanics that would've been exploited in a far more interesting way if the AI used a larger set of rules, or Fuzzy Logic, for example. To get more into detail upon the last consideration, one should consider that the game is over as soon as all the Collectible items are picked up, as there's no time limit. This would mean that the fewer the Collectibles on the board, the more should the player with the lower score focus on stealing points, and the more should the player with the higher score focus into reaching the last Collectibles while avoiding collisions with the other player. With Fuzzy Logic, such kind of behaviour would've definitely added a nice touch of pseudo-nondeterminism, by controlling how much would the AI be inclined onto chasing the player instead of focusing solely on the Collectible items. A similar thing could've been done if there were no walls, as the AI (or the player) would've had to consider if and how to push the other player over the edge to achieve victory. In a time limited version, the winning player might also have to try to run away as much as he can to avoid getting his points stolen before the end. While all these gameplay related conundrums are fascinating, the current version of the Rule Base System behaves in a much more trivial way, as described earlier. Nonetheless, the possibilities that lie within such a simple environment are definitely worth discussing.

o **RBS: Implementation**

The implementation for this AI has been basically done by building a rule based system on top of a NavMeshAgent, a particular kind of intelligent agent based on the Unity.AI module. The advantage of this agent is the ability to use pathfinding, with a discreetly efficient obstacle avoidance system that can also be tweaked, in order to make it more or less performing. What had to be done for this project was mostly tampering with the agent's destination, in order to change it dynamically according to the rule based system's database. In order to make it avoid the Danger items, a NavMeshObstacle component was added to the objects, so that they'd be automatically avoided.

To make the agent move properly the physical properties of the agent's Rigidbody component was set as kinematic, as any collision ruined the agent's motion, but a problem caused by the kinematic property was that collisions weren't happening anymore. To solve this last issue the kinematic property is turned off on collision, the impact force is applied, and the kinematic property is turned back on.

o **RBS: Review**

Rule Based Systems are deterministic, and as such they behave in ways that are predictable; to make the gameplay a bit more interesting, the position of all the items on the board are generated randomly. Nonetheless, after a while it is easy to guess and exploit the agent's behaviour. Using controlled randomness could be a way to make the RBS more unpredictable, but finding something to make random is not easy.

Play testing against this AI has been a good experience: the AI can be challenging, and picking all the Collectible items up before it does isn't easy sometimes, although this may depend on how they are positioned on the board.

Coding the AI has also been a relatively easy task, and testing it didn't require much time. It's also easy to debug, and identifying flaws within its behaviour is really simple, since we can always infer the expected result.

Overall, if somebody wants quick results as fast as he can from an artificial intelligence, Rule Based Systems are a fine choice. And now, for something completely different...

- **The Second AI: Reinforcement Learning**

o **ML-Agent: Design**

Prior to designing the ML-Agent more documentation needed to be read first, as while there was a beforehand knowledge of the concepts behind Reinforcement Learning, what was missing was the knowledge regarding how to implement this technique in Unity. Luckily, the Unity SDK for ML-Agents has been really helpful. Thanks to this amazing toolkit, creating a learning environment for the ML-Agent revealed itself to be far easier than expected. The focus shifted mostly on reward systems, and how to tune in the hyperparameters, which is more a concern for the implementation process and will be discussed later.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image006.jpg)

The agent's learning system is quite simple: on every step it takes an action, and collects observation, while increasing or decreasing the reward. At the end of the episode the AI collects the reward, which represents the end result of all the actions that have been taken during the episode, taking into account all the observation data that has been collected.

In this case the actions are merely two: the movement on the horizontal axis and the movement on the vertical axis. The observations that are collected during the training are the following:

§ The current target, which is either the closest Collectible item or the Enemy player if the latter has a higher score than the Agent and is closer than the closest collectible item

§ The Agent's position

§ The Enemy's position

§ The closest threat, which is either the closest Danger item or the Enemy player if the latter has a lower score than the Agent and is closer than the closest Danger item

§ The Agent's speed on the x Axis and the z Axis

§ The Enemy score

§ The Agent's score

As the match is played through and reward conditions are fulfilled, a function is called to increase the reward accordingly, before feeding the final result to the machine when the episode is finished. The training is all done using the RBS agent as the enemy, in order to improve the learning process.

While all of this sounds ideal, more issues than expected have emerged during the implementation, and required the learning environment to be tweaked by changing the reward system in a way that would've allow to increase the learning rate faster.

o **ML-Agent: Implementation**

This part can be really tough, although setting up the environment is actually easy thanks to the Unity SDK for ML-Agents, as has been mentioned before. Once everything is set up, running the code may not immediately produce the expected results. In fact, within the first few hours the ML-Agent did not seem to be learning much, as can be seen from the screenshot below.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image008.jpg)

Considering that the reward ranges from a minimum of -0.25 to a maximum of 1, it could be said that the ML-Agent's first training session wasn't exactly successful. After reading a bit more into the documentation, realization came: what actually happened was an issue with tuning the hyperparameters.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image010.jpg)Hyperparameters are basically a set of values inside a configuration .yaml file that is used to control the learning process of the ML-Agent.

In the case at hand, such values were the ones shown in the picture beside.

Although not all the variables within the configuration file have been tweaked, some of them have been manipulated.

In order to understand what has changed and how it did improve the learning environment or the quality of the data that was gathered, each variable's characteristics is briefly described in the following section.

§ **Trainer**: The first insight that was gathered was about the Trainer algorithm, the Proximal Policy Optimization, PPO for short. The objective is to learn a Policy that allows the Agent to obtain the highest reward possible. Although the Agent was mostly trained with this algorithm, another one has been tried as well out of curiosity, to see if there were visible performance changes. This other algorithm is known as Soft Actor Critic, SAC for short, and apparently it's built in a way that relies less on hyperparameters. As its makers' from Berkeley describe it, it's an off-policy model-free reinforcement learning algorithm. While there are slightly noticeable differences between SAC and PPO, the latter has been selected for the demonstration, since at the point when SAC's properties were understood, the Agent had been trained enough with PPO to a point where it had finally started performing decently.

§ **Batch Size**: this number represents the experiences that are used for one iteration of a gradient descent update. While at first increasing this value as much as possible seemed like a good idea, after a few tries it was understood that it doesn't strictly improve the agent's performance beyond a certain threshold, which is also dependent on what method of collecting observation we're using. If the method is discreet, the threshold is smaller, if it's continuous we have to use a larger number.

§ **Beta**: this value sets the strength of the entropy regularization, which basically increases the policy's randomness. This ensures that agents properly explore the action space during training. Increasing this allows more random actions: it should be adjusted such that the entropy slowly decreases alongside increases in reward. If entropy drops too quickly, beta needs to be increased. If it drops too slowly, it has to be decreased.

§ **Buffer Size**: this corresponds to how many agent observations, actions and rewards should be collected before the model is updated. This should be a multiple of the **Batch Size**. Since a larger Buffer Size corresponds to more stable training updates, this number was increased accordingly.

§ **Epsilon**: this corresponds to the acceptable threshold of divergence between the old and new policies during gradient descent updating. Setting this value small will result in more stable updates, but will also slow the training process. While at first it was thought that decreasing this value would've been a good idea, excessively slow training proved otherwise.

§ **Hidden Units**: These units are in each fully connected layer of the neural network. For simple problems where the correct action is a straightforward combination of the observation inputs, this should be small. For problems where the action is a very complex interaction between the observation variables, this should be larger.

§ **Lambda**: it's a parameter used when calculating the Generalized Advantage Estimate (GAE). This can be thought of as how much the agent relies on its current value estimate when calculating an updated value estimate. Low values correspond to relying more on the current value estimate (which can be high bias), and high values correspond to relying more on the actual rewards received in the environment (which can be high variance). The parameter provides a trade-off between the two, and the right value can lead to a more stable training process.

§ **Learning Rate**: this represents the strength of each gradient descent update step. Typically decreased if the training is unstable, and the reward does not consistently increase. After a few trials, it was decided to avoid decreasing this value too much, since the reward did increase over time, although slowly.

§ **Learning Rate Schedule**: this value corresponds to how the learning rate is changed over time. For PPO, the recommended learning rate is usually set to linear in order to make it decay until Max Steps is reached, so that learning converges in a more stable way. Since it wasn't possible to infer how much training time would've been required, a decision was made to set the rate to constant instead, until a plateau was reached in the gradient descent. Once this condition was met, the training was paused, and the variable was set back to linear before continuing for the last steps.

§ **Max Steps**: this corresponds to how many steps of the simulation (multiplied by frame-skip) are run during the training process. This value has just been repeatedly increased until the Agent reached a reasonable level of skill.

§ **Memory Size**: The size of the memory an agent must keep. Used for training with a recurrent neural network.

§ **Normaliz**e: this value is used to decide whether or not normalization is applied to the vector observation inputs. This normalization is based on the running average and variance of the vector observation. Normalization can be helpful in cases with complex continuous control problems, but may be harmful with simpler discrete control problems.

§ **Number of Epochs**: it is the number of passes through the experience buffer during gradient descent. The larger the Batch Size, the larger it is acceptable to make this. Decreasing this ensures more stable updates, at the cost of slower learning. Since the certainty of stable updates was preferred, it has been decided to keep this value low.

§ **Number of Layers**: this represents how many hidden layers are present after the observation input, or after the CNN (Convolutional Neural Network) encoding of the visual observation. For simple problems, fewer layers are likely to train faster and more efficiently. More layers may be necessary for more complex control problems. Technically this number shouldn't ever be too large, so it has been left it as it was.

§ **Time Horizon**: this corresponds to the amount of steps of experience to collect per-agent before adding it to the experience buffer. When this limit is reached before the end of an episode, a value estimate is used to predict the overall expected reward from the agent's current state. As such, this parameter trades off between a less biased, but higher variance estimate (long time horizon) and more biased, but less varied estimate (short time horizon). In cases where there are frequent rewards within an episode, or episodes are prohibitively large, a smaller number can be more ideal. This number should be large enough to capture all the important behaviour within a sequence of an agent's actions. Since the Agent is trained against a RBS, the episodes aren't much long as the game ends as soon as all the Collectible items are collected; I've thus opted for a reasonably small sized Time Horizon.

§ **Sequence Length**: Defines how long the sequences of experiences must be while training in Recurrent Neural Networks.

§ **Summary Frequency**: It is used to decide how often, in steps, we want to save training statistics. This determines the number of data points shown by TensorBoard.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image012.jpg)Playing around with the hyperparameters is not a simple task, and although one might wish this to be the last obstacle into building a functioning learning environment, there are more things that one has to keep in mind in order to get the most out of what reinforcement learning has to offer.

Among the notions that have been gathered during this process there are some little tweaks that improve the learning rate.

§ **Parallel training**

Using multiple test environments together is an incredibly efficient way of scaling things up: multiple instances can be initialised together, while sharing the same brain. The only object that needs to be unique is the Academy, the core of the entire learning process.

§ **Customizing the training environment**

Although it may not feel right at first, training the ML-Agent in the final environment may not be the right choice. It may be costly, and it may also be underperforming. For example, as it has been mentioned before, this game does have walls, and during the first training session, one odd behaviour of the agent was constantly rolling against walls. To avoid this, in the training environment the walls have been removed, and a negative reward had been set if the agent fell off the board, in order to make the agent avoid the positions were the walls were supposed to be. This did positively improve the learning process. Once it was clear that the agent wouldn't have ever crashed itself against the walls, these have been put back in the training environment.

§ **Training against another AI**

As mentioned earlier, during the first training session the agent has been trained against the RBS AI. After the agent reached a decent level of skill, it has been tested against itself, to see if there were any relevant improvements in performance, but this second training session hasn't proved particularly successful. Nonetheless, training against the RBS drastically improved the ML-Agent's behaviour, as it has even learned how to exploit once in a while certain behavioural patterns of the Rule Based System.

o **ML-Agent: Review**

Training the ML-Agent hasn't been easy, nor simple, although more confidence has been acquired on the subject. Training requires time, and rewards have to be set with extreme attention to details, in order to avoid a badly trained AI. The most troubling part is doubtlessly the hyperparameters' tuning process. It's not immediate, and to know if an agent has been finely tuned it is necessary to let it train for a decent amount of time and play-test it. This process can be discomforting if the agent's reward system isn't coded properly, as one may find out about unexpected behaviour only after there is a compiled brain, an .nn file.

Nonetheless, with a proper grasp of the concept it's not hard to set the hyperparameters, and once the proper values are found, all that has to be done is waiting: after a certain amount of steps if the environment has been coded correctly the Cumulative Reward will start to increase.

To monitor this we need to use TensorBoard, an application based on TensorFlow that allows users to gather data on the learning process, even as the process is still going on. This tool allows us to access the information on a local server through any browser. The data is displayed according to the following statistics.

§ **Cumulative Reward**

The general trend in reward should consistently increase over time. Small ups and downs are to be expected. Depending on the complexity of the task, a significant increase in reward may not present itself until millions of steps into the training process. As can be seen, the ML-Agent for Roll-A-Ball started to display significant increases only after 2 million steps.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image014.png)

§ **Episode Length**

Although it is strictly related to the kind of training environment we're using, episode length is another useful way of controlling if there are significant changes in the Agent's expected behaviour. Since the episodes length is supposed to be decreasing over time (as the Agent learns how to collect the items on the board), this information is particularly useful in this case.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image016.jpg)

§ **Policy Loss**

These values will oscillate during training. Generally they should be less than 1.0.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image018.jpg)

§ **Value Estimate**

These values should increase as the cumulative reward increases. They correspond to how much future reward the agent predicts itself receiving at any given point.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image020.jpg)

§ **Value Loss**

These values will increase as the reward increases, and then should decrease once reward becomes stable. As can be seen in the following picture, the training curve tends to increase slowly, but surely.

![](file:///C:/Users/Ridam/AppData/Local/Temp/msohtmlclip1/01/clip_image022.jpg)

**Conclusions**

Building a classic Rule Based System may be way easier than using Reinforcement Learning, but the potentiality of Learning Agents is impressive, and shouldn't be discarded just because there are simpler solutions that are easier to implement. Before making such a decision, one should always evaluate the pros and cons of both AIs. If the goal we have in mind is to have an efficient problem-solving robot, RBS can doubtlessly come in handy, but if the goal is to make a human-like foe that actually behaves in a realistic non-deterministic way, Reinforcement Learning is a better choice, given that we have enough time to train it.

A heavily trained Agent can also offer more challenging matches, as it can learn tricks to exploit environments and is also useful to spot bugs and design errors in the game. As a matter of fact, a trick that the agent learned to exploit in Roll-A-Ball is putting itself on the trajectory of a losing enemy, to be pushed in the direction of Collectible items. 

**References**

GitHub. (2019). *Unity-Technologies/ml-agents*. [online] Available at: https://github.com/Unity-Technologies/ml-agents/tree/master/docs [Accessed 30 Oct. 2019].

Seita, D. (2019). *Soft Actor Critic---Deep Reinforcement Learning with Real-World Robots*. [online] The Berkeley Artificial Intelligence Research Blog. Available at: https://bair.berkeley.edu/blog/2018/12/14/sac/ [Accessed 30 Oct. 2019].
