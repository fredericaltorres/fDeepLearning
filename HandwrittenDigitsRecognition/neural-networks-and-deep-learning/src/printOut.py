import gzip
import pickle
import numpy as np

def save_mnist_to_text(input_file="../data/mnist.pkl.gz", output_file="./mnist_output.txt"):
    try:
        # Load the MNIST dataset from the pickle file
        with gzip.open(input_file, 'rb') as f:
            # The MNIST pickle file contains three tuples: training, validation, and test sets
            train_set, valid_set, test_set = pickle.load(f, encoding='latin1')
            
            # Each set is a tuple of (data, labels)
            train_data, train_labels = train_set
            valid_data, valid_labels = valid_set
            test_data, test_labels = test_set
            
            # Combine all data
            ##all_data = np.vstack((train_data, valid_data, test_data))
            all_data = np.vstack((train_data))
            
            # Open output file and write each image as a comma-separated line
            with open(output_file, 'w') as out_f:
                for image in all_data:
                    # Convert pixel values to strings and join with commas
                    line = ','.join(str(pixel) for pixel in image)
                    out_f.write(line + '\n')
                    
        print(f"Successfully processed {len(all_data)} images")
        print(f"Data shape: {all_data.shape}")
        print(f"Output written to: {output_file}")
        
    except FileNotFoundError:
        print(f"Error: Could not find input file {input_file}")
    except Exception as e:
        print(f"An error occurred: {str(e)}")

if __name__ == "__main__":
    save_mnist_to_text()