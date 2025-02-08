FRED REPO IS
 https://github.com/unexploredtest/neural-networks-and-deep-learning

# Export image to bmp
 python .\printOut.py

# Digit Recognition Video
Neural Networks 009: Stochastic Gradient Descent (https://www.youtube.com/watch?v=HSJgWG0fcVw)


# Embedding Space

"a fluffy blue creature roamed the verdant forest"

vector 1, just the word no context + position in phrase

3 matrix (query and key live in the same space)
- query vector/matrix, smaller 64 len, Is there any adjective around current word?
- key matrix, when they are adjective, answer yes

fluffy and blue are adjective.

dot product to determine if 2 vector point in the same direction.
if > 0 align, 0 perpendiculaire, -1 pointing in opposite direction

softmax() normalize value between 0..1, create the weight

Videos:
    Visualizing transformers and attention | Talk for TNG Big Tech Day '24 (https://www.youtube.com/watch?v=KJtZARuO3JY&t=1477s)
    Transformers (how LLMs work) explained visually | DL5 (https://www.youtube.com/watch?v=wjZofJX0v4M)
    Let's build GPT: from scratch, in code, spelled out. (https://www.youtube.com/watch?v=kCc8FmEb1nY)

Blogs:
    Attention in Transformers (https://medium.com/lazy-by-design/attention-in-transformers-3blue1brown-fbba4e826ab4)


Authors:
    michael nielsen deep learning (http://neuralnetworksanddeeplearning.com/)