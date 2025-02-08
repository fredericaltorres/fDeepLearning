import gzip
import pickle
import numpy as np
import os

def delete_file_if_exists(file_path):
    try:
        if os.path.exists(file_path):
            os.remove(file_path)
            print(f"File {file_path} was deleted successfully")
        else:
            print(f"File {file_path} does not exist")
    except Exception as e:
        print(f"Error deleting file {file_path}: {str(e)}")

def save_mnist_to_text(input_file="../data/mnist.pkl.gz", output_file="./mnist_output.txt"):
    try:
        delete_file_if_exists(output_file)
        with gzip.open(input_file, 'rb') as f: # Load the MNIST dataset from the pickle file
            # The MNIST pickle file contains three tuples: training, validation, and test sets
            train_set, valid_set, test_set = pickle.load(f, encoding='latin1')
            
            # Each set is a tuple of (data, labels)
            train_data, train_labels = train_set
            valid_data, valid_labels = valid_set
            test_data, test_labels = test_set
            
            # Combine all data
            ##all_data = np.vstack((train_data, valid_data, test_data))
            all_data = np.vstack((train_data))
            all_labels = np.concatenate((train_labels, valid_labels, test_labels))

            #print(f"Successfully processed {len(all_labels)} labels")
            print(f"Dataset composition:")
            print(f"Training set: {len(train_labels)},{len(train_data)},{len(train_set)}, images")
            print(f"Output written to: {output_file}")
            
            # Open output file and write each image as a comma-separated line
            x = 0
            with open(output_file, 'w') as out_f:
                for image in all_data:
                    label = all_labels[x]
                    out_f.write(f"{label}\n")
                    line = ','.join(str(pixel) for pixel in image) # Convert pixel values to strings and join with commas
                    out_f.write(line + '\n')
                    x = x + 1
                    
        print(f"Successfully processed {len(all_data)} images")
        print(f"Data shape: {all_data.shape}")
        print(f"Output written to: {output_file}")
        
    except FileNotFoundError:
        print(f"Error: Could not find input file {input_file}")
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    save_mnist_to_text()