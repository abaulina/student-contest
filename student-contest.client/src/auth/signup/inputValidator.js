export function isInputValid(userCredentials, setErrors) {
	return (
		validateName(userCredentials.firstName, setErrors) &&
		validateLastName(userCredentials.lastName, setErrors) &&
		validateEmail(userCredentials.email, setErrors) &&
		validatePassword(userCredentials.password, setErrors)
	);
}

const isFirstNameValid = (firstName) => {
	const regex = /^[a-zA-ZаЯЁёА-я]+([-]?\s?[a-zA-ZЁёА-я])?$/;
	return regex.test(firstName);
};

const isLastNameValid = (lastName) => {
	const regex = /^[a-zA-ZаЯЁёА-я]+([-]?\s?[a-zA-ZЁёА-я])?$/;
	return regex.test(lastName);
};

const validateName = (firstName, setErrors) => {
	if (!isFirstNameValid(firstName)) {
		setErrors((prevState) => ({
			...prevState,
			firstName: 'First name is invalid'
		}));
		return false;
	}
	return true;
};

const validateLastName = (lastName, setErrors) => {
	if (!isLastNameValid(lastName)) {
		setErrors((prevState) => ({
			...prevState,
			lastName: 'Last name is invalid'
		}));
		return false;
	}
	return true;
};

const isCorrectEmailFormat = (email) => {
	const regex =
		/^(([^<>()[\].,;:\s@"]+(\.[^<>()[\].,;:\s@"]+)*)|(".+"))@(([^<>()[\].,;:\s@"]+\.)+[^<>()[\].,;:\s@"]{2,})$/i;
	return regex.test(email);
};

const validateEmail = (email, setErrors) => {
	if (!isCorrectEmailFormat(email)) {
		setErrors((prevState) => ({
			...prevState,
			email: 'Email is invalid'
		}));
		return false;
	}
	return true;
};

const validatePassword = (password, setErrors) => {
	if (password.length < 8) {
		setErrors((prevState) => ({
			...prevState,
			password: 'Password is invalid. It must be at least 8 characters'
		}));
		return false;
	}
	return true;
};
