#region Copyright(c) Data Systems International, Inc. 2020
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
// 
// Filename: Extensions.cs
#endregion

namespace DefaultNamespace {

    public static class Extensions {

        public static string toOnOffText(this bool value) => value ? "ON" : "OFF";

    }

}