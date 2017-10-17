# Custom Vision Lab

In this lab, we will learn how to recognize different objects in the image using [Microsoft Custom Vision API](https://docs.microsoft.com/en-us/azure/cognitive-services/custom-vision-service/).

## Come up with a Problem

You can train Custom Vision API to recognize different objects in a picure by using relatively small training dataset (10-50 images). However, it can only be
used in cases when the object is quite clearly visible, and occupies most of the picture. For more complex object detection scenarious and for object localizaton,
you need to use more complex techniques based on neural networks.

Keeping this consideration in mind, try to come up with a problem for object recognition. You shoud be able to find 20-30 similar images of each object category
in Bing Image Search (or otherwise have those training images available).

Good examples of problems can be:

1. Recognizing different soda types: Fanta, Cola, Sprite
2. Recognizing logos of different brands: Microsoft, Intel, Google
3. Recognizing type of goods in the supermarket
4. Recognizing different electronic components
5. Recognizing animals
6. ...

## Getting training images

One of the ways to get training images is to use Bing Image Search. To automatically download top Bing Image Search results, you can use the
[imdownload.exe](./imdownload.exe) utulity provided.
You can call it from the command line in the following way:
```
imdownload <bing key> "<search term>" [<image mask with #> <number of images>]
```
You need Bing Search API key to use it - please obtain your own key [here](https://azure.microsoft.com/en-us/try/cognitive-services/?api=bing-web-search-api)
or through your cloud subscription (by creating Cognitive Services -> Search object). You can also try to use the following key, but just for the sake of this lab: `46e12b3bfaa042128641ce04f72f7bbc`.

For example, to download 30 pictures of yellow cats, use the command:
```
imdownload xxxxxx "Yellow Cat" Cat# 30
```

After downloading images for each category, you need to clean up the training data:

 * browse all images and delete those that do not correctly repserent the category
 * put some of the images (2-5) aside, to be used as testing data

## Train the Model

To create and train the model, go to http://customvision.ai and do the following:

1. Select **New Project** and fill in project name, etc.
2. For each category, upload training images and assign them to a certain tag which represents category
3. Select **Train Model** and observe the values of **precision** and **recall**. Those values reflect the accuracy of the model (the bigger the numbers - the better), and allow estimating percentage of false positive and false negative errors. 
4. For testing the model, select **Quick Test** and upload testing images (which we have set aside earlier).

## Testing Model through Telegram Bot

In order to test the model on real pictures, it is advices to have [Telegram](http://telegram.org) messenger installed on your phone. It is available for all
platforms, and also through [web version](https://web.telegram.org/). 

To test your model, add `@MyCustomVisionBot` as a new user in Telegram. 

In order to use your model, you need to provide the bot with your model id and prediction key. In the control panel of Custom Vision service, select `Performance -> Prediction Url`. Look for the following informtion:

* Service URL, which will look like this: `https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/xxxxxx-xxxx-xxxx-xxxx-xxxxx/url`
* Prediction Key (alphanumeric sequence)

First of all, send to the bot your model id (characters `xxxxxx-xxxx-xxxx-xxxx-xxxxx` in the URL above) and prediction key, separated by space. 

After your bot has been given model id and prediction key, you can send it images to recogize. It is very convenient way to test a model against images that you take with your phone camera.

I hope you have enjoyed the exercise!
