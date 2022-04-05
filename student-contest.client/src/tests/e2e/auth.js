import { Selector } from 'testcafe';
import { validUser } from '../data/inputData';

export const login = async (t) => {
	await t
		.typeText('#floatingEmail', validUser.email)
		.typeText('#floatingPassword', validUser.password)
		.click(Selector('button').withText('Submit'));
};

export const generateEmail = () => {
	var chars = 'abcdefghijklmnopqrstuvwxyz1234567890';
	var string = '';
	for (var ii = 0; ii < 15; ii++) {
		string += chars[Math.floor(Math.random() * chars.length)];
	}
	return string + '@domain.com';
};
