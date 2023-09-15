import os



def read_template(template_name :str, replace_dict :dict) -> str:
    """
    Reads template from file, replaces variables in message and returns html code
    Args:
        template_name (str): template file name
        replace_dict (dict): dictionary where keys are names of variables in the template and args are strings that should be placed in message instead of the keys
    Return:
        HTML message
    """

    with open(f'{os.path.dirname(__file__)}\\{template_name}', 'r', encoding='utf-8') as file:
        message = file.read()
    for key in replace_dict.keys():
        message = message.replace(key, replace_dict[key])
    return message
