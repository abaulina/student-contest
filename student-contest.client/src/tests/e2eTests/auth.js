import { ClientFunction, Selector } from 'testcafe';
import { validUser } from '../data/inputData';

const setLocalStorageItem = ClientFunction((key, value) =>
	localStorage.setItem(key, value)
);

const setLocalStorageTestUser = async () => {
	const users = [];
	users.push(validUser);
	await setLocalStorageItem('Users', JSON.stringify(users));
};

export const useCookies = async () => {
	const setCookie = ClientFunction(() => {
		document.cookie = 'userEmail=test@example.com';
	});
	await setLocalStorageTestUser();
	await setCookie();
};

export const login = async (t) => {
	await setLocalStorageTestUser();
	await t
		.typeText('#floatingEmail', validUser.email)
		.typeText('#floatingPassword', validUser.password)
		.click(Selector('button').withText('Submit'));
};

export const loginAfterSignup = async (t) => {
	await t
		.typeText('#floatingEmail', validUser.email)
		.typeText('#floatingPassword', validUser.password)
		.click(Selector('button').withText('Submit'));
};
